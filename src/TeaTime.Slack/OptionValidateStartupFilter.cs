namespace TeaTime.Slack
{
    //TODO: Move to better location

    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class OptionValidateStartupFilter<T> : IStartupFilter where T : class, new()
    {
        private readonly IOptions<T> _options;
        private readonly ILogger<OptionValidateStartupFilter<T>> _logger;

        public OptionValidateStartupFilter(IOptions<T> options, ILogger<OptionValidateStartupFilter<T>> logger)
        {
            _options = options;
            _logger = logger;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            try
            {
                // Try can get the values
                _ = _options.Value;
            }
            catch (OptionsValidationException e)
            {
                var errorMessage = e.Failures.First();

                _logger.LogCritical("Fatal config error: " + errorMessage);

                throw;
            }

            return next;
        }
    }
}
