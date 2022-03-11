using Microsoft.Extensions.Logging;
using WindowsBouncer.Persistence;

namespace WindowsBouncer.Core
{
    public class PersistenceHandler : Handler
    {
        private readonly IDataService _dataService;
        private readonly ILogger<PersistenceHandler> _logger;

        public PersistenceHandler(IDataService dataService, ILogger<PersistenceHandler> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        public override IList<SecurityEvent> Handle(IList<SecurityEvent> securityEventList)
        {
            foreach (var securityEvent in securityEventList)
            {
                var result = _dataService.AddLoginAttempt(new LoginAttempt
                {
                    Ip = HelperMethods.IpStringToLong(securityEvent.Ip),
                    Login = !string.IsNullOrEmpty(securityEvent.Domain) ? $"{securityEvent.Domain}\\{securityEvent.User}" : securityEvent.User,
                    Date = securityEvent.CreatedAt
                });

                if (result == DbOperationResult.Success)
                {
                    _logger.LogInformation($"Entry with IP {securityEvent.Ip} added to database");
                }
                else if (result == DbOperationResult.AlreadyExists)
                {
                    _logger.LogInformation($"Entry with IP {securityEvent.Ip} already exists in database");
                }
                else if (result == DbOperationResult.Failure)
                {
                    _logger.LogError($"Error adding entry with IP {securityEvent.Ip} to database");
                }
            }
            
            return base.Handle(securityEventList);
        }

        public override void Dispose()
        {
            _dataService.Close();
            base.Dispose();
        }
    }
}
