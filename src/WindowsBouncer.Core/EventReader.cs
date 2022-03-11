using System.Diagnostics;

namespace WindowsBouncer.Core
{
    public class EventReader
    {
        public IList<SecurityEvent> GetFailedLoginAttempts(DateTime? since = null)
        {
            EventLog log = new EventLog("Security");
            var entries = log.Entries.Cast<EventLogEntry>()
                                     .Where(e => e.EventID == Constants.WINDOWS_FAILEDAUDIT
                                            && e.ReplacementStrings.Length >= 20);

            if (since != null)
            {
                entries = entries.Where(e => e.TimeGenerated >= since);
            }

            return entries.Select(e => new SecurityEvent
            {
                Ip = e.ReplacementStrings[19],
                User = e.ReplacementStrings[5],
                Domain = e.ReplacementStrings[6],
                CreatedAt = e.TimeGenerated
            }).ToList();
        }
    }
}
