using Microsoft.AspNetCore.Mvc;
using VmRebuildApi.Jobs;
using VmRebuildApi.Models;
using VmRebuildApi.Services;

namespace VmRebuildApi.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class VmController : ControllerBase
    {
        private readonly IJobStore jobStore;
        private readonly RebuildJobWorker rebuildJobWorker;
        private readonly ILogger<VmController> logger;
        public VmController(IJobStore store, RebuildJobWorker worker, ILogger<VmController> log)
        {
            jobStore = store;
            rebuildJobWorker = worker;
            logger = log;
        }

        [HttpPost("vm/rebuild")]
        public IActionResult Rebuild([FromBody] RebuildVmRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.VmName) || string.IsNullOrWhiteSpace(request.TemplateName))
                return BadRequest("VmName and TemplateName are required.");

            var job = jobStore.Create();
            logger.LogInformation("Queued rebuild job {JobId} for VM {Vm}", job.JobId, request.VmName);

            // Fire-and-Forget: possibility to replace a queue f.i. RabbitMQ
            _ = Task.Run(() => rebuildJobWorker.RunAsync(job.JobId, request, cancellationToken));

            return Accepted(new { jobId = job.JobId, statusUrl = $"/api/v1/jobs/{job.JobId}" });
        }

        [HttpGet("jobs/{jobId:guid}")]
        public IActionResult GetJob(Guid jobId)
        {
            if (!jobStore.TryGet(jobId, out var job))
                return NotFound();

            return Ok(job);
        }
    }
}
