namespace TeaTime.Slack.Commands;

using System.Threading;
using System.Threading.Tasks;
using CommandRouter.Commands;
using CommandRouter.Results;
using Common.Models.Data;
using Models;
using Models.SlashCommands;
using Services;

public abstract class BaseCommand(ISlackService slackService) : Command
{
    protected ISlackService SlackService { get; } = slackService;

    protected static ICommandResult Ok()
    {
        return new StringResult("");
    }

    protected async Task<CommandContext> GetContextAsync(CancellationToken cancellationToken = default)
    {
        var command = GetCommand();
        var user = await GetOrCreateUserAsync(command, cancellationToken);
        var room = await GetOrCreateRoomAsync(command, user.Id, cancellationToken);

        return new CommandContext(command, user, room);
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

    private Task<User> GetOrCreateUserAsync(SlashCommand slashCommand, CancellationToken cancellationToken = default)
    {
        return SlackService.GetOrCreateUserAsync(slashCommand.UserId, slashCommand.GetUserName(), cancellationToken);
    }

    private Task<Room> GetOrCreateRoomAsync(SlashCommand slashCommand, long userId,
        CancellationToken cancellationToken = default)
    {
        return SlackService.GetOrCreateRoomAsync(slashCommand.ChannelId, slashCommand.ChannelName, userId,
            cancellationToken);
    }
}
