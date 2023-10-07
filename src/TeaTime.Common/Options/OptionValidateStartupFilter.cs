namespace TeaTime.Common.Options
{
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class OptionValidateStartupFilter<T> : IStartupAction where T : class, new()
    {
        private readonly IOptions<T> _options;
        private readonly ILogger<OptionValidateStartupFilter<T>> _logger;

        public OptionValidateStartupFilter(IOptions<T> options, ILogger<OptionValidateStartupFilter<T>> logger)
        {
            _options = options;
            _logger = logger;
        }

        public string Name => "OptionValidateStartupFilter<" + typeof(T).Name + ">";

        public void Execute()
        {
            try
            {
                // Try can get the values
                _ = _options.Value;
            }
            catch (OptionsValidationException e)
            {
                var errorMessage = e.Failures.First();

                _logger.LogCritical("Fatal config error: {Message}", errorMessage);

                throw;
            }
        }
    }
}
