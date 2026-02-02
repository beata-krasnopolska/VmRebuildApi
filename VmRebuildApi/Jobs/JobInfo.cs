namespace VmRebuildApi.Jobs
{
    public sealed class JobInfo
    {
        public Guid JobId { get; set; }
        public JobState State { get; set; }
        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? FinishedAt { get; set; }

        public int? ExitCode { get; set; }
        public string? Error { get; set; }
        public string? Output { get; set; }
    }
}
