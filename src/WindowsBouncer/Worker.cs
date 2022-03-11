using WindowsBouncer.Core;

namespace WindowsBouncer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private Timer _timer;
        private readonly EventReader _eventReader;
        private readonly Handler _firewallHandler;
        private readonly Handler _persistenceHandler;

        public Worker(
            ILogger<Worker> logger, 
            EventReader eventReader, 
            HandlerResolver handlerResolver)
        {
            _logger = logger;
            _eventReader = eventReader;
            _firewallHandler = handlerResolver("FirewallHandler");
            _persistenceHandler = handlerResolver("PersistenceHandler");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service is started.");
            var args = new WorkArgs { CancellationToken = stoppingToken, JobMode = JobMode.FullScan };
            DoWork(args);
            args.JobMode = JobMode.Interval;
            _timer = new Timer(new TimerCallback(DoWork), args, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _firewallHandler.Dispose();
            await base.StopAsync(cancellationToken);
        }

        private void DoWork(object args)
        {
            _logger.LogInformation($"Work started.");
            var workArgs = (WorkArgs)args;
            CancellationToken cancellationToken = workArgs.CancellationToken;

            Task t = Task.Run(() =>
            {
                try
                {
                    IList<SecurityEvent> newEvents = null;

                    switch (workArgs.JobMode)
                    {
                        case JobMode.FullScan:
                            _eventReader.GetFailedLoginAttempts();
                            break;
                        case JobMode.Interval:
                            newEvents = _eventReader.GetFailedLoginAttempts(DateTime.Now.Subtract(TimeSpan.FromMinutes(1)));
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    if (newEvents == null || newEvents.Count == 0)
                    {
                        _logger.LogInformation($"No new events available.");
                        return;
                    }

                    _logger.LogInformation($"{newEvents.Count} new events available.");
                    _firewallHandler.SetNext(_persistenceHandler);
                    _firewallHandler.Handle(newEvents);
                    _logger.LogInformation($"Event handling complete.");
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error: {e.Message}\r\n{e.StackTrace}");
                }

            }, cancellationToken);

            t.Wait(cancellationToken);
        }
    }
}