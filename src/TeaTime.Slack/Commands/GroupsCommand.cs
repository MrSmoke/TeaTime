namespace TeaTime.Slack.Commands;

using System.Threading.Tasks;
using CommandRouter.Attributes;
using CommandRouter.Results;
using Common.Abstractions;
using Common.Features.RoomItemGroups.Commands;
using Common.Features.RoomItemGroups.Queries;
using MediatR;
using Models.SlashCommands;
using Resources;
using Services;

[CommandPrefix("groups")]
public class GroupsCommand(
    ISlackService slackService,
    IMediator mediator,
    IIdGenerator<long> idGenerator)
    : BaseCommand(slackService)
{
    [Command("add")]
    public async Task<ICommandResult> AddGroup(string name)
    {
        var context = await GetContextAsync();

        var command = new CreateRoomItemGroupCommand(
            Id: await idGenerator.GenerateAsync(),
            RoomId: context.Room.Id,
            Name: name,
            CurrentUserId: context.User.Id
        );

        await mediator.Send(command);

        return Response(ResponseStrings.GroupAdded(name), ResponseType.User);
    }

    [Command("remove")]
    public async Task<ICommandResult> RemoveGroup(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Response(ErrorStrings.RemoveGroup_BadArguments(), ResponseType.User);

        var context = await GetContextAsync();

        var group = await mediator.Send(new GetRoomItemGroupByNameQuery(context.Room.Id, context.User.Id, name));
        if (group == null)
            return Response(ErrorStrings.RemoveGroup_GroupInvalidName(name), ResponseType.User);

        var command = new DeleteRoomItemGroupCommand(
            GroupId: group.Id,
            CurrentUserId: context.User.Id
        );

        await mediator.Send(command);

        return Response(ResponseStrings.GroupRemoved(name), ResponseType.User);
    }
}
