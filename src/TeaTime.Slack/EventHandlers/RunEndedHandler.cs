namespace TeaTime.Slack.EventHandlers
{
    using System.Collections.Generic;
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

            var orders = notification.Orders.ToList();

            // Create a dictionary of all usernames in the current order (including the runner)
            var userIds = new HashSet<long>(orders.Select(o => o.User.Id)) {notification.RunnerUserId};

            var usernames = (await Task.WhenAll(userIds.Select(async uid =>
                    new KeyValuePair<long, string>(uid, await GetSlackUsername(uid)))))
                .ToDictionary(k => k.Key, v => v.Value);

            var messageBuilder = new StringBuilder();

            messageBuilder
                .Append("Congratulations ")
                .Append(usernames[notification.RunnerUserId])
                .Append(" you drew the short straw :cup_with_straw:. \n\nHere's the order:\n");

            for (var i = 0; i < orders.Count; i++)
            {
                var o = orders[i];

                messageBuilder
                    .Append("- ")
                    .Append(usernames[o.User.Id])
                    .Append(": ")
                    .Append(o.Option.Name);

                if (i < orders.Count - 1)
                    messageBuilder.Append("\n");
            }

            var data = new SlashCommandResponse(messageBuilder.ToString(), ResponseType.Channel);

            await _slackApiClient.PostResponseAsync(callbackData.ResponseUrl, data).ConfigureAwait(false);
        }

        private async Task<string> GetSlackUsername(long userId)
        {
            var runnerSlackId = await _linkRepository.GetLinkAsync(userId, LinkType.User).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(runnerSlackId))
                return StringHelpers.SlackUserId(runnerSlackId);

            var runner = await _userRepository.GetAsync(userId).ConfigureAwait(false);

            return runner?.DisplayName ?? "Unknown";
        }
    }
}