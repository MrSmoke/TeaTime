namespace TeaTime.Slack.Commands
{
    using System.Threading.Tasks;
    using CommandRouter.Attributes;
    using CommandRouter.Results;
    using Common.Abstractions;
    using Common.Features.RoomItemGroups.Commands;
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
            var slashCommand = GetCommand();

            var user = await GetOrCreateUser(slashCommand).ConfigureAwait(false);
            var room = await GetOrCreateRoom(slashCommand, user.Id).ConfigureAwait(false);

            var command = new CreateRoomItemGroupCommand(
                id: await _idGenerator.GenerateAsync().ConfigureAwait(false),
                roomId: room.Id,
                name: name,
                userId: user.Id
            );

            await _mediator.Send(command).ConfigureAwait(false);

            return Response(ResponseStrings.GroupAdded(name), ResponseType.User);
        }
    }
}