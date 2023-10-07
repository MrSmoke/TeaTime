namespace TeaTime.Common
{
    using System.Threading.Tasks;
    using Abstractions;
    using MediatR;

    public class EventPublisher : IEventPublisher
    {
        private readonly IMediator _mediator;

        public EventPublisher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task PublishAsync<T>(T @event) where T : IEvent
        {
            return _mediator.Publish(@event);
        }
    }
}
