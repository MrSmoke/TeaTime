namespace TeaTime.Slack.Commands
{
    using System.Threading.Tasks;
    using CommandRouter.Commands;
    using CommandRouter.Results;
    using Common.Models.Data;
    using Models;
    using Models.SlashCommands;
    using Services;

    public abstract class BaseCommand(ISlackService slackService) : Command
    {
        protected async Task<CommandContext> GetContextAsync()
        {
            var command = GetCommand();
            var user = await GetOrCreateUser(command);
            var room = await GetOrCreateRoom(command, user.Id);

            return new CommandContext(command, user, room);
        }

        private Task<User> GetOrCreateUser(SlashCommand slashCommand)
        {
            return slackService.GetOrCreateUser(slashCommand.UserId, slashCommand.UserName);
        }

        private Task<Room> GetOrCreateRoom(SlashCommand slashCommand, long userId)
        {
            return slackService.GetOrCreateRoom(slashCommand.ChannelId, slashCommand.ChannelName, userId);
        }

        protected SlashCommand GetCommand() => (SlashCommand)Context.Items[Constants.CommandContextKeys.SlashCommand];

        protected ICommandResult Response(string? text, ResponseType responseType)
        {
            return Response(new SlashCommandResponse(text, responseType));
        }

        protected ICommandResult Response(SlashCommandResponse response)
        {
            return StringResult(SlackJsonSerializer.Serialize(response));
        }

        protected static ICommandResult Empty { get; } = new StringResult(string.Empty);
    }
}
