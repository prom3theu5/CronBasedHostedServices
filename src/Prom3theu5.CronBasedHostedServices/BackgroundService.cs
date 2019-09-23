using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Prom3theu5.CronBasedHostedServices
{
    /// <summary>
    /// Class BackgroundService.
    /// Implements the <see cref="Microsoft.Extensions.Hosting.IHostedService" />
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.IHostedService" />
    public abstract class BackgroundService : IHostedService
    {
        /// <summary>
        /// The executing task
        /// </summary>
        private Task _executingTask;
        /// <summary>
        /// The stopping CTS
        /// </summary>
        private readonly CancellationTokenSource _stoppingCts =
            new CancellationTokenSource();

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>Task.</returns>
        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        /// <summary>
        /// stop as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        /// <summary>
        /// execute as an asynchronous operation.
        /// </summary>
        /// <param name="stoppingToken">The stopping token.</param>
        protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                await Process();

                await Task.Delay(5000, stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        /// <summary>
        /// Processes this instance.
        /// </summary>
        /// <returns>Task.</returns>
        protected abstract Task Process();
    }
}
