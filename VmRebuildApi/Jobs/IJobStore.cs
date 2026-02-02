namespace VmRebuildApi.Jobs
{
    public interface IJobStore
    {
        JobInfo Create();
        bool TryGet(Guid jobId, out JobInfo job);
        void Update(JobInfo job);
    }
}
