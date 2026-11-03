namespace TeaTime.Common;

using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using MediatR;

public class EventPublisher(IMediator mediator) : IEventPublisher
{
    public Task PublishAsync<T>(T eventModel, CancellationToken cancellationToken = default) where T : IEvent
    {
        return mediator.Publish(eventModel, cancellationToken);
    }
}
