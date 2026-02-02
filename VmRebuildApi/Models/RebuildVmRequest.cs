namespace VmRebuildApi.Models
{
    public sealed class RebuildVmRequest
    {
        public string VmName { get; init; } = default!;
        public string TemplateName { get; init; } = default!;
        public string? Datastore { get; init; }
        public string? Cluster { get; init; }
        public string? NetworkName { get; init; }
        public bool PowerOn { get; init; } = true;
    }
}
