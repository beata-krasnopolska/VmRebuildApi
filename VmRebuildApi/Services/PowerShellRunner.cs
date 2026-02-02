using System.Diagnostics;
using System.Text;

namespace VmRebuildApi.Services
{
    public class PowerShellRunner
    {
        private readonly ILogger<PowerShellRunner> _log;

        public PowerShellRunner(ILogger<PowerShellRunner> log) => _log = log;

        public async Task<(int exitCode, string output)> RunAsync(
            string scriptPath,
            IReadOnlyDictionary<string, string> args,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            if (!File.Exists(scriptPath))
                throw new FileNotFoundException($"Script not found: {scriptPath}");

            // Building parameters: -VmName "X" -TemplateName "Y" ...
            var psArgs = new StringBuilder();
            psArgs.Append("-NoProfile -ExecutionPolicy Bypass -File ");
            psArgs.Append('"').Append(scriptPath).Append('"');

            foreach (var (k, v) in args)
            {
                psArgs.Append(' ')
                      .Append('-').Append(k)
                      .Append(' ')
                      .Append('"').Append(v.Replace("\"", "\\\"")).Append('"');
            }

            var psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = psArgs.ToString(),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using var p = new Process { StartInfo = psi };

            _log.LogInformation("Starting PowerShell: {Args}", psi.Arguments);
            p.Start();

            var stdOutTask = p.StandardOutput.ReadToEndAsync();
            var stdErrTask = p.StandardError.ReadToEndAsync();

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(timeout);

            try
            {
                await p.WaitForExitAsync(timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                try { if (!p.HasExited) p.Kill(entireProcessTree: true); } catch { /* ignore */ }
                throw new TimeoutException($"PowerShell timed out after {timeout}.");
            }

            var stdout = await stdOutTask;
            var stderr = await stdErrTask;

            var combined = string.IsNullOrWhiteSpace(stderr) ? stdout : $"{stdout}\n[stderr]\n{stderr}";
            return (p.ExitCode, combined);
        }
    }
}
