namespace TeaTime.Common;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Behaviour for executing all <see cref="ICommandPreProcessor{TRequest}"/> instances before handling a request
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class CommandPreProcessorBehavior<TRequest, TResponse>(
    IEnumerable<ICommandPreProcessor<TRequest>> preProcessors,
    ILogger<CommandPreProcessorBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        foreach (var processor in preProcessors)
        {
            logger.LogDebug("Running {PreProcessor} for command {Command}",
                processor.GetType().Name, typeof(TRequest).Name);

            await processor.ProcessAsync(request, cancellationToken);
        }

        return await next(cancellationToken);
    }
}
