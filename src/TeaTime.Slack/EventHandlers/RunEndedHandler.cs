namespace TeaTime.Slack.EventHandlers;

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
using Models.SlashCommands;
using Resources;

internal class RunEndedHandler(
    ISlackApiClient slackApiClient,
    ILinkRepository linkRepository,
    IUserRepository userRepository,
    ILogger<RunEndedHandler> logger)
    : INotificationHandler<RunEndedEvent>
{
    public async Task Handle(RunEndedEvent notification, CancellationToken cancellationToken)
    {
        if (!notification.TryGetCallbackState(out var callbackData))
        {
            logger.LogWarning("RunEndedNotification did not contain any callback data");
            return;
        }

        var orders = notification.Orders.ToList();

        // Create a dictionary of all usernames in the current order (including the runner)
        var orderUserIds = orders.WhereNotNull(o => o.User).Select(o => o.Id);
        HashSet<long> userIds = [.. orderUserIds, notification.RunnerUserId];

        var usernames = (await Task.WhenAll(userIds.Select(async uid =>
                new KeyValuePair<long, string>(uid, await GetSlackUsernameAsync(uid, cancellationToken)))))
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
                .Append(StringHelpers.EscapeMarkdown(o.Option?.Name ?? "unknown"));

            if (i < orders.Count - 1)
                messageBuilder.Append('\n');
        }

        var data = new SlashCommandResponse(messageBuilder.ToString(), ResponseType.Channel);

        await slackApiClient.PostResponseAsync(callbackData.ResponseUrl, data, cancellationToken);
    }

    private async Task<string> GetSlackUsernameAsync(long userId, CancellationToken cancellationToken = default)
    {
        var runnerSlackId = await linkRepository.GetLinkAsync(userId, LinkType.User, cancellationToken);

        if (!string.IsNullOrWhiteSpace(runnerSlackId))
            return StringHelpers.SlackUserId(runnerSlackId);

        var runner = await userRepository.GetAsync(userId, cancellationToken);

        return StringHelpers.EscapeMarkdown(runner?.DisplayName ?? "Unknown");
    }
}
