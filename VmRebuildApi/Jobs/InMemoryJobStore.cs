
using System.Collections.Concurrent;

namespace VmRebuildApi.Jobs
{
    public class InMemoryJobStore : IJobStore
    {
        private readonly ConcurrentDictionary<Guid, JobInfo> _jobs = new();
        public JobInfo Create()
        {
            var job = new JobInfo { JobId = Guid.NewGuid(), State = JobState.Queued };
            _jobs[job.JobId] = job;
            return job;
        }

        public bool TryGet(Guid jobId, out JobInfo job)
        {
            var result = _jobs.TryGetValue(jobId, out job!);
            return result;
        }

        public void Update(JobInfo job) => _jobs[job.JobId] = job;
    }
}
