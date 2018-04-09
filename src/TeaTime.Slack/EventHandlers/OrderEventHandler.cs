﻿namespace TeaTime.Slack.EventHandlers
{
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
    using Models;
    using Models.Responses;
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
            var slackUserId = await GetSlackUserId(notification.UserId).ConfigureAwait(false);
            if (slackUserId == null)
                return;

            await SendMessage(notification, ResponseStrings.RunUserJoined(slackUserId)).ConfigureAwait(false);
        }

        public async Task Handle(OrderOptionChangedEvent notification, CancellationToken cancellationToken)
        {
            var slackUserId = await GetSlackUserId(notification.UserId).ConfigureAwait(false);
            if (slackUserId == null)
                return;

            var options = await _optionsRepository.GetManyAsync(new[] {notification.PreviousOptionId, notification.OptionId})
                    .ConfigureAwait(false);

            var oDict = options.ToDictionary(o => o.Id);

            var message = ResponseStrings.RunUserOrderChanged(
                slackUserId,
                oDict[notification.PreviousOptionId].Name,
                oDict[notification.OptionId].Name);

            await SendMessage(notification, message).ConfigureAwait(false);
        }

        private async Task<string> GetSlackUserId(long userId)
        {
            var slackUserId = await _linkRepository.GetLinkAsync(userId, LinkType.User).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(slackUserId))
                return slackUserId;

            _logger.LogError("Could not get slack user link for: {UserId}", userId);
            return null;

        }

        private async Task SendMessage(Event evt, string message)
        {
            if (!evt.TryGetCallbackState(out var callbackData))
                return;

            await _slackApiClient.PostResponseAsync(callbackData.ResponseUrl,
                    new SlashCommandResponse(message, ResponseType.Channel))
                .ConfigureAwait(false);
        }
    }
}
