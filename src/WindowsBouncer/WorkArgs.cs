namespace WindowsBouncer
{
    internal class WorkArgs
    {
        public JobMode JobMode { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
