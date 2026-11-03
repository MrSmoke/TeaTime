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

public class OrderEventHandler :
    INotificationHandler<OrderPlacedEvent>,
    INotificationHandler<OrderOptionChangedEvent>
{
    private readonly ISlackApiClient _slackApiClient;
    private readonly ILinkRepository _linkRepository;
    private readonly ILogger<OrderEventHandler> _logger;
    private readonly IOptionsRepository _optionsRepository;

    public OrderEventHandler(ISlackApiClient slackApiClient, ILinkRepository linkRepository, ILogger<OrderEventHandler> logger, IOptionsRepository optionsRepository)
    {
        _slackApiClient = slackApiClient;
        _linkRepository = linkRepository;
        _logger = logger;
        _optionsRepository = optionsRepository;
    }

    public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        var slackUserId = await GetSlackUserIdAsync(notification.UserId, cancellationToken);
        if (slackUserId == null)
            return;

        await SendMessageAsync(notification, ResponseStrings.RunUserJoined(slackUserId), ResponseType.Channel,
            cancellationToken);
    }

    public async Task Handle(OrderOptionChangedEvent notification, CancellationToken cancellationToken)
    {
        var slackUserId = await GetSlackUserIdAsync(notification.UserId, cancellationToken);
        if (slackUserId == null)
            return;

        var options = await _optionsRepository.GetManyAsync(new[]
        {
            notification.PreviousOptionId,
            notification.OptionId
        }, cancellationToken);

        var oDict = options.ToDictionary(o => o.Id);

        var message = ResponseStrings.RunUserOrderChanged(
            oDict[notification.PreviousOptionId].Name,
            oDict[notification.OptionId].Name);

        await SendMessageAsync(notification, message, ResponseType.User, cancellationToken);
    }

    private async Task<string?> GetSlackUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        var slackUserId = await _linkRepository.GetLinkAsync(userId, LinkType.User, cancellationToken);
        if (!string.IsNullOrWhiteSpace(slackUserId))
            return slackUserId;

        _logger.LogError("Could not get slack user link for: {UserId}", userId);
        return null;
    }

    private async Task SendMessageAsync(BaseEvent evt, string message, ResponseType responseType,
        CancellationToken cancellationToken = default)
    {
        if (!evt.TryGetCallbackState(out var callbackData))
            return;

        await _slackApiClient.PostResponseAsync(callbackData.ResponseUrl,
            new SlashCommandResponse(message, responseType), cancellationToken);
    }
}
