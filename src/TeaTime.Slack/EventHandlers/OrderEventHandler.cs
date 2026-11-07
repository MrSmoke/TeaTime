namespace TeaTime.Slack.EventHandlers;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client;
using Common.Abstractions;
using Common.Abstractions.Data;
using Common.Features.Orders.Events;
using Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Models.SlashCommands;
using Resources;

public class OrderEventHandler(
    ISlackApiClient slackApiClient,
    ILinkRepository linkRepository,
    ILogger<OrderEventHandler> logger,
    IOptionsRepository optionsRepository) :
    INotificationHandler<OrderPlacedEvent>,
    INotificationHandler<OrderOptionChangedEvent>,
    INotificationHandler<OrderDeletedEvent>
{
    public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        var slackUserId = await GetSlackUserIdAsync(notification.UserId, cancellationToken);
        if (slackUserId == null)
            return;

        await SendMessageAsync(notification,
            ResponseStrings.RunUserJoined(slackUserId),
            ResponseType.Channel,
            cancellationToken);
    }

    public async Task Handle(OrderOptionChangedEvent notification, CancellationToken cancellationToken)
    {
        var slackUserId = await GetSlackUserIdAsync(notification.UserId, cancellationToken);
        if (slackUserId == null)
            return;

        long[] optionIds = [notification.PreviousOptionId, notification.OptionId];
        var options = await optionsRepository.GetManyAsync(optionIds, cancellationToken);

        var oDict = options.ToDictionary(o => o.Id);

        var message = ResponseStrings.RunUserOrderChanged(
            oDict[notification.PreviousOptionId].Name,
            oDict[notification.OptionId].Name);

        await SendMessageAsync(notification, message, ResponseType.User, cancellationToken);
    }

    public async Task Handle(OrderDeletedEvent notification, CancellationToken cancellationToken)
    {
        var slackUserId = await GetSlackUserIdAsync(notification.Order.UserId, cancellationToken);
        if (slackUserId == null)
            return;

        await SendMessageAsync(notification,
            ResponseStrings.RunUserLeft(slackUserId),
            ResponseType.Channel,
            cancellationToken);
    }

    private async Task<string?> GetSlackUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        var slackUserId = await linkRepository.GetLinkAsync(userId, LinkType.User, cancellationToken);
        if (!string.IsNullOrWhiteSpace(slackUserId))
            return slackUserId;

        logger.LogError("Could not get slack user link for: {UserId}", userId);
        return null;
    }

    private async Task SendMessageAsync(BaseEvent evt, string message, ResponseType responseType,
        CancellationToken cancellationToken = default)
    {
        if (!evt.TryGetCallbackState(out var callbackData))
            return;

        await slackApiClient.PostResponseAsync(
            callbackData.ResponseUrl,
            new SlashCommandResponse(message, responseType),
            cancellationToken);
    }
}
