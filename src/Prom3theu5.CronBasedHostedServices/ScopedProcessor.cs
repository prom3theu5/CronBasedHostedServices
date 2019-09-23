using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Prom3theu5.CronBasedHostedServices
{
    /// <summary>
    /// Class ScopedProcessor.
    /// Implements the <see cref="Prom3theu5.CronBasedHostedServices.BackgroundService" />.
    /// </summary>
    /// <seealso cref="Prom3theu5.CronBasedHostedServices.BackgroundService" />
    public abstract class ScopedProcessor : BackgroundService
    {
        /// <summary>
        /// The service scope factory
        /// </summary>
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Processes the in scope.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>Task.</returns>
        public abstract Task ProcessInScope(IServiceProvider serviceProvider);

        /// <summary>
        /// Initialises a new instance of the <see cref="ScopedProcessor" /> class.
        /// </summary>
        /// <param name="serviceScopeFactory">The service scope factory.</param>
        protected ScopedProcessor(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// Processes this instance.
        /// </summary>
        /// <returns>Task.</returns>
        protected override async Task Process()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                await ProcessInScope(scope.ServiceProvider);
            }
        }
    }
}
