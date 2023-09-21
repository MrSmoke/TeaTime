namespace TeaTime.Common
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using MediatR;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Behavior for executing all <see cref="ICommandPreProcessor{TRequest}"/> instances before handling a request
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class CommandPreProcessorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<ICommandPreProcessor<TRequest>> _preProcessors;
        private readonly ILogger<CommandPreProcessorBehavior<TRequest, TResponse>> _logger;

        public CommandPreProcessorBehavior(IEnumerable<ICommandPreProcessor<TRequest>> preProcessors,
            ILogger<CommandPreProcessorBehavior<TRequest, TResponse>> logger)
        {
            _preProcessors = preProcessors;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            foreach (var processor in _preProcessors)
            {
                _logger.LogDebug("Running ICommandPreProcessor for command {Command}", typeof(TRequest).Name);

                await processor.ProcessAsync(request, cancellationToken);
            }

            return await next();
        }
    }
}
