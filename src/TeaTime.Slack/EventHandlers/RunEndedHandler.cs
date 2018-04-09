namespace TeaTime.Slack.EventHandlers
{
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Features.Runs.Events;
    using MediatR;
    using Models.Requests;
    using Models.Responses;
    using Resources;

    internal class RunEndedHandler : INotificationHandler<RunEndedEvent>
    {
        public async Task Handle(RunEndedEvent notification, CancellationToken cancellationToken)
        {
            if (!notification.State.TryGetValue(Constants.SlashCommand, out var slashCommandJson))
                return;

            var slashCommand = SlackJsonSerializer.Deserialize<SlashCommand>(slashCommandJson);

            if (string.IsNullOrWhiteSpace(slashCommand.ResponseUrl))
                return;

            var message =
                $"Congratulations {StringHelpers.SlackUserId(slashCommand.UserId)} you drew the short straw :cup_with_straw: Here's the order: ";

            foreach (var o in notification.Orders)
            {
                message += $"\n{o.User.DisplayName}: {o.Option.Name}";
            }

            var data = new SlashCommandResponse(message, ResponseType.Channel);

            var json = SlackJsonSerializer.Serialize(data);

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(slashCommand.ResponseUrl,
                    new StringContent(json, Encoding.UTF8, "application/json"),
                    cancellationToken).ConfigureAwait(false);

                //todo: log errors
            }
        }
    }
}