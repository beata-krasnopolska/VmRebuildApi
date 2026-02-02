using VmRebuildApi.Jobs;
using VmRebuildApi.Models;

namespace VmRebuildApi.Services
{
    public class RebuildJobWorker
    {
        private readonly IJobStore _store;
        private readonly PowerShellRunner _ps;
        private readonly ILogger<RebuildJobWorker> _log;
        private readonly IWebHostEnvironment _env;

        public RebuildJobWorker(IJobStore store, PowerShellRunner ps, ILogger<RebuildJobWorker> log, IWebHostEnvironment env)
        {
            _store = store;
            _ps = ps;
            _log = log;
            _env = env;
        }

        public async Task RunAsync(Guid jobId, RebuildVmRequest req, CancellationToken cancellationToken)
        {
            if (!_store.TryGet(jobId, out var job))
                return;

            job.State = JobState.Running;
            job.StartedAt = DateTime.UtcNow;
            _store.Update(job);

            try
            {
                var scriptPath = Path.Combine(_env.ContentRootPath, "Scripts", "Rebuild-VM.ps1");

                var args = new Dictionary<string, string>
                {
                    ["VmName"] = req.VmName,
                    ["TemplateName"] = req.TemplateName,
                    ["Datastore"] = req.Datastore ?? "",
                    ["Cluster"] = req.Cluster ?? "",
                    ["NetworkName"] = req.NetworkName ?? "",
                    ["PowerOn"] = req.PowerOn ? "true" : "false"
                };

                var (exit, output) = await _ps.RunAsync(scriptPath, args, timeout: TimeSpan.FromMinutes(20), cancellationToken);

                job.ExitCode = exit;
                job.Output = output;
                job.State = exit == 0 ? JobState.Succeeded : JobState.Failed;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Job failed: {JobId}", jobId);
                job.State = JobState.Failed;
                job.Error = ex.Message;
            }
            finally
            {
                job.FinishedAt = DateTime.UtcNow;
                _store.Update(job);
            }
        }
    }
}
