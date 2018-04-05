namespace TeaTime.Slack.Commands
{
    using System.Threading.Tasks;
    using CommandRouter.Commands;
    using CommandRouter.Results;
    using Common.Models.Data;
    using Models.Requests;
    using Models.Responses;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Services;

    public abstract class BaseCommand : Command
    {
        private readonly ISlackService _slackService;

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        protected BaseCommand(ISlackService slackService)
        {
            _slackService = slackService;
        }

        protected Task<User> GetOrCreateUser(SlashCommand slashCommand)
        {
            return _slackService.GetOrCreateUser(slashCommand.UserId, slashCommand.UserName);
        }

        protected Task<Room> GetOrCreateRoom(SlashCommand slashCommand, long userId)
        {
            return _slackService.GetOrCreateRoom(slashCommand.ChannelId, slashCommand.ChannelName, userId);
        }

        protected SlashCommand GetCommand() => (SlashCommand)Context.Items["SLASHCOMMAND"];

        protected ICommandResult Response(string text, ResponseType responseType)
        {
            return Response(new SlashCommandResponse(text, responseType));
        }

        protected ICommandResult Response(SlashCommandResponse response)
        {
            return StringResult(JsonConvert.SerializeObject(response, JsonSettings));
        }
    }
}