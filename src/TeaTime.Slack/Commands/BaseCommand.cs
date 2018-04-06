namespace TeaTime.Slack.Commands
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CommandRouter.Commands;
    using CommandRouter.Results;
    using Common.Models.Data;
    using Models.Requests;
    using Models.Responses;
    using Services;

    public abstract class BaseCommand : Command
    {
        private readonly ISlackService _slackService;

        protected BaseCommand(ISlackService slackService)
        {
            _slackService = slackService;
        }

        protected Dictionary<string, string> GetState(Dictionary<string, string> state)
        {
            state.Add(Constants.SlashCommand, SlackJsonSerializer.Serialize(GetCommand()));

            return state;
        }

        protected Dictionary<string, string> GetState()
        {
            return GetState(new Dictionary<string, string>());
        }

        protected Task<User> GetOrCreateUser(SlashCommand slashCommand)
        {
            return _slackService.GetOrCreateUser(slashCommand.UserId, slashCommand.UserName);
        }

        protected Task<Room> GetOrCreateRoom(SlashCommand slashCommand, long userId)
        {
            return _slackService.GetOrCreateRoom(slashCommand.ChannelId, slashCommand.ChannelName, userId);
        }

        protected SlashCommand GetCommand() => (SlashCommand)Context.Items[Constants.SlashCommand];

        protected ICommandResult Response(string text, ResponseType responseType)
        {
            return Response(new SlashCommandResponse(text, responseType));
        }

        protected ICommandResult Response(SlashCommandResponse response)
        {
            return StringResult(SlackJsonSerializer.Serialize(response));
        }
    }
}