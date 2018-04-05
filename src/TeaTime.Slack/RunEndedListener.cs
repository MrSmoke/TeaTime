namespace TeaTime.Slack
{
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Features.Runs.Events;
    using MediatR;
    using Models.Requests;
    using Models.Responses;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Resources;

    public class RunEndedHandler : INotificationHandler<RunEndedEvent>
    {
        public async Task Handle(RunEndedEvent notification, CancellationToken cancellationToken)
        {
            if (!notification.State.TryGetValue("SlashCommand", out var slashCommandJson))
                return;

            var slashCommand = JsonConvert.DeserializeObject<SlashCommand>(slashCommandJson);

            if (string.IsNullOrWhiteSpace(slashCommand.ResponseUrl))
                return;

            var message =
                $"Congratulations {StringHelpers.SlackUserId(slashCommand.UserId)} you drew the short straw :cup_with_straw: Here's the order: ";

            foreach (var o in notification.Orders)
            {
                message += $"\n{o.UserId}: {o.OptionId}";
            }

            var data = new SlashCommandResponse(message, ResponseType.Channel);

            var json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

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