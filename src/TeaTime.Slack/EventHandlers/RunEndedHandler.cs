namespace TeaTime.Slack.EventHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Client;
    using Common.Features.Runs.Events;
    using MediatR;
    using Models.Responses;
    using Resources;

    internal class RunEndedHandler : INotificationHandler<RunEndedEvent>
    {
        private readonly ISlackApiClient _slackApiClient;

        public RunEndedHandler(ISlackApiClient slackApiClient)
        {
            _slackApiClient = slackApiClient;
        }

        public async Task Handle(RunEndedEvent notification, CancellationToken cancellationToken)
        {
            if (!notification.TryGetCallbackState(out var callbackData))
                return;

            var message =
                $"Congratulations {StringHelpers.SlackUserId(callbackData.UserId)} you drew the short straw :cup_with_straw: Here's the order: ";

            foreach (var o in notification.Orders)
            {
                message += $"\n{o.User.DisplayName}: {o.Option.Name}";
            }

            var data = new SlashCommandResponse(message, ResponseType.Channel);

            await _slackApiClient.PostResponseAsync(callbackData.ResponseUrl, data).ConfigureAwait(false);
        }
    }
}