using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NCrontab;

namespace Prom3theu5.CronBasedHostedServices
{
    /// <summary>
    /// Class ScheduledProcessor.
    /// Implements the <see cref="Prom3theu5.CronBasedHostedServices.ScopedProcessor" />
    /// </summary>
    /// <seealso cref="Prom3theu5.CronBasedHostedServices.ScopedProcessor" />
    public abstract class ScheduledProcessor : ScopedProcessor
    {
        /// <summary>
        /// The schedule
        /// </summary>
        private readonly CrontabSchedule _schedule;
        /// <summary>
        /// The next run
        /// </summary>
        private DateTime _nextRun;
        /// <summary>
        /// Gets the schedule.
        /// </summary>
        /// <value>The schedule.</value>
        protected abstract string Schedule { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ScheduledProcessor"/> class.
        /// </summary>
        /// <param name="serviceScopeFactory">The service scope factory.</param>
        protected ScheduledProcessor(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
            _schedule = CrontabSchedule.Parse(Schedule);
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
        }

        /// <summary>
        /// execute as an asynchronous operation.
        /// </summary>
        /// <param name="stoppingToken">The stopping token.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.Now;
                if (now > _nextRun)
                {
                    await Process();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                }

                await Task.Delay(5000, stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested);
        }
    }
}