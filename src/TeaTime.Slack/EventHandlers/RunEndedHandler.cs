namespace TeaTime.Slack.EventHandlers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Client;
    using Common.Abstractions.Data;
    using Common.Extensions;
    using Common.Features.Runs.Events;
    using Common.Models;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Models.Responses;
    using Resources;

    internal class RunEndedHandler : INotificationHandler<RunEndedEvent>
    {
        private readonly ISlackApiClient _slackApiClient;
        private readonly ILinkRepository _linkRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RunEndedHandler> _logger;

        public RunEndedHandler(ISlackApiClient slackApiClient,
            ILinkRepository linkRepository,
            IUserRepository userRepository,
            ILogger<RunEndedHandler> logger)
        {
            _slackApiClient = slackApiClient;
            _linkRepository = linkRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Handle(RunEndedEvent notification, CancellationToken cancellationToken)
        {
            if (!notification.TryGetCallbackState(out var callbackData))
            {
                _logger.LogWarning("RunEndedNotification did not contain any callback data");
                return;
            }

            var orders = notification.Orders.ToList();

            // Create a dictionary of all usernames in the current order (including the runner)
            var userIds = new HashSet<long>(orders.WhereNotNull(o => o.User).Select(o => o.Id))
            {
                notification.RunnerUserId
            };

            var usernames = (await Task.WhenAll(userIds.Select(async uid =>
                    new KeyValuePair<long, string>(uid, await GetSlackUsername(uid)))))
                .ToDictionary(k => k.Key, v => v.Value);

            var messageBuilder = new StringBuilder();

            messageBuilder
                .Append("Congratulations ")
                .Append(usernames[notification.RunnerUserId])
                .Append(" you drew the short straw :cup_with_straw: \n\nHere's the order:\n");

            for (var i = 0; i < orders.Count; i++)
            {
                var o = orders[i];

                if (o.User is null || !usernames.TryGetValue(o.User.Id, out var username))
                    username = "unknown";

                messageBuilder
                    .Append("- ")
                    .Append(username)
                    .Append(": ")
                    .Append(o.Option?.Name ?? "unknown");

                if (i < orders.Count - 1)
                    messageBuilder.Append('\n');
            }

            var data = new SlashCommandResponse(messageBuilder.ToString(), ResponseType.Channel);

            await _slackApiClient.PostResponseAsync(callbackData.ResponseUrl, data);
        }

        private async Task<string> GetSlackUsername(long userId)
        {
            var runnerSlackId = await _linkRepository.GetLinkAsync(userId, LinkType.User);

            if (!string.IsNullOrWhiteSpace(runnerSlackId))
                return StringHelpers.SlackUserId(runnerSlackId);

            var runner = await _userRepository.GetAsync(userId);

            return runner?.DisplayName ?? "Unknown";
        }
    }
}
