namespace TeaTime.Slack
{
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Features.Runs.Events;
    using MediatR;

    public class RunEndedHandler : INotificationHandler<RunEndedEvent>
    {
        private readonly IRunEventListener _eventListener;

        public RunEndedHandler(IRunEventListener eventListener)
        {
            _eventListener = eventListener;
        }

        public Task Handle(RunEndedEvent notification, CancellationToken cancellationToken)
        {
            _eventListener.Trigger(notification);

            return Task.CompletedTask;
        }
    }
}