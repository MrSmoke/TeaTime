namespace TeaTime.Slack.EventHandlers
{
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Client;
    using Common.Abstractions.Data;
    using Common.Features.Runs.Events;
    using Common.Models;
    using MediatR;
    using Models.Responses;
    using Resources;

    internal class RunEndedHandler : INotificationHandler<RunEndedEvent>
    {
        private readonly ISlackApiClient _slackApiClient;
        private readonly ILinkRepository _linkRepository;
        private readonly IUserRepository _userRepository;

        public RunEndedHandler(ISlackApiClient slackApiClient, ILinkRepository linkRepository, IUserRepository userRepository)
        {
            _slackApiClient = slackApiClient;
            _linkRepository = linkRepository;
            _userRepository = userRepository;
        }

        public async Task Handle(RunEndedEvent notification, CancellationToken cancellationToken)
        {
            if (!notification.TryGetCallbackState(out var callbackData))
                return;

            var runnerSlackId = await _linkRepository.GetLinkAsync(notification.RunnerUserId, LinkType.User).ConfigureAwait(false);

            string slackRunnerIdStr;
            if (string.IsNullOrWhiteSpace(runnerSlackId))
            {
                var runner = await _userRepository.GetAsync(notification.RunnerUserId).ConfigureAwait(false);
                slackRunnerIdStr = runner.DisplayName;
            }
            else
            {
                slackRunnerIdStr = StringHelpers.SlackUserId(runnerSlackId);
            }

            var messageBuilder = new StringBuilder();

            messageBuilder
                .Append("Congratulations ")
                .Append(slackRunnerIdStr)
                .Append(" you drew the short straw :cup_with_straw:. Here's the order:")
                .Append("\n");

            var orders = notification.Orders.ToList();

            for (var i = 0; i < orders.Count; i++)
            {
                var o = orders[i];

                messageBuilder
                    .Append("- ")
                    .Append(o.User.DisplayName)
                    .Append(": ")
                    .Append(o.Option.Name);

                if (i < orders.Count - 1)
                    messageBuilder.Append("\n");
            }

            var data = new SlashCommandResponse(messageBuilder.ToString(), ResponseType.Channel);

            await _slackApiClient.PostResponseAsync(callbackData.ResponseUrl, data).ConfigureAwait(false);
        }
    }
}