namespace WindowsBouncer.Core
{
    public class SecurityEvent
    {
        public string Ip { get; set; }
        public string Domain { get; set; }
        public string User { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
