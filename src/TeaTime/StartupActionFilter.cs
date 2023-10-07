namespace TeaTime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;

    public class StartupActionFilter : IStartupFilter
    {
        private readonly IReadOnlyCollection<IStartupAction> _actions;
        private readonly ILogger<StartupActionFilter> _logger;

        public StartupActionFilter(IEnumerable<IStartupAction> actions, ILogger<StartupActionFilter> logger)
        {
            _actions = actions.ToList();
            _logger = logger;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            _logger.LogDebug("Running {Count} startup actions", _actions.Count);

            foreach (var action in _actions)
            {
                _logger.LogDebug("Running startup action: {StartupActionName}", action.Name);

                action.Execute();
            }

            return next;
        }
    }
}
