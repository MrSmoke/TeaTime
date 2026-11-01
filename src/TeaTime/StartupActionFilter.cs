namespace TeaTime;

using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

public class StartupActionFilter(IEnumerable<IStartupAction> actions, ILogger<StartupActionFilter> logger)
    : IStartupFilter
{
    private readonly IStartupAction[] _actions = actions.ToArray();

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        logger.LogDebug("Running {Count} startup actions", _actions.Length);

        foreach (var action in _actions)
        {
            logger.LogDebug("Running startup action: {StartupActionName}", action.Name);

            action.Execute();
        }

        return next;
    }
}
