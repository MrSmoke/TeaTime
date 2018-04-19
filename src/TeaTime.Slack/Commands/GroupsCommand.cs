namespace TeaTime.Slack.Commands
{
    using System.Threading.Tasks;
    using CommandRouter.Attributes;
    using CommandRouter.Results;
    using Common.Abstractions;
    using Common.Features.RoomItemGroups.Commands;
    using Common.Features.RoomItemGroups.Queries;
    using MediatR;
    using Models.Responses;
    using Resources;
    using Services;

    [CommandPrefix("groups")]
    public class GroupsCommand : BaseCommand
    {
        private readonly IMediator _mediator;
        private readonly IIdGenerator<long> _idGenerator;

        public GroupsCommand(ISlackService slackService, IMediator mediator, IIdGenerator<long> idGenerator) : base(slackService)
        {
            _mediator = mediator;
            _idGenerator = idGenerator;
        }

        [Command("add")]
        public async Task<ICommandResult> AddGroup(string name)
        {
            var context = await GetContextAsync().ConfigureAwait(false);

            var command = new CreateRoomItemGroupCommand(
                id: await _idGenerator.GenerateAsync().ConfigureAwait(false),
                roomId: context.Room.Id,
                name: name,
                userId: context.User.Id
            );

            await _mediator.Send(command).ConfigureAwait(false);

            return Response(ResponseStrings.GroupAdded(name), ResponseType.User);
        }

        [Command("remove")]
        public async Task<ICommandResult> RemoveGroup(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Response(ErrorStrings.RemoveGroup_BadArguments(), ResponseType.User);

            var context = await GetContextAsync().ConfigureAwait(false);

            var group = await _mediator.Send(new GetRoomItemGroupByNameQuery(context.Room.Id, context.User.Id, name)).ConfigureAwait(false);
            if(group == null)
                return Response(ErrorStrings.RemoveGroup_GroupInvalidName(name), ResponseType.User);

            var command = new DeleteRoomItemGroupCommand(
                groupId: group.Id,
                userId: context.User.Id
            );

            await _mediator.Send(command).ConfigureAwait(false);

            return Response(ResponseStrings.GroupRemoved(name), ResponseType.User);
        }
    }
}