namespace TeaTime.Slack.Commands
{
    using System.Threading.Tasks;
    using CommandRouter.Attributes;
    using CommandRouter.Results;
    using Common.Abstractions;
    using Common.Features.Options.Commands;
    using Common.Features.RoomItemGroups.Queries;
    using MediatR;
    using Models.Responses;
    using Resources;
    using Services;

    [CommandPrefix("options")]
    public class OptionsCommand : BaseCommand
    {
        private readonly IMediator _mediator;
        private readonly IIdGenerator<long> _idGenerator;

        public OptionsCommand(ISlackService slackService, IMediator mediator, IIdGenerator<long> idGenerator) : base(slackService)
        {
            _mediator = mediator;
            _idGenerator = idGenerator;
        }

        [Command("add")]
        public async Task<ICommandResult> AddOption(string groupName, string optionName)
        {
            var slashCommand = GetCommand();

            var user = await GetOrCreateUser(slashCommand).ConfigureAwait(false);
            var room = await GetOrCreateRoom(slashCommand, user.Id).ConfigureAwait(false);

            var group = await _mediator.Send(new GetRoomItemGroupByNameQuery(
                roomId: room.Id,
                userId: user.Id,
                name: groupName)).ConfigureAwait(false);

            if (group == null)
                return Response(ErrorStrings.AddOption_GroupInvalidName(groupName), ResponseType.User);

            var command =
                new CreateOptionCommand(
                    id: await _idGenerator.GenerateAsync().ConfigureAwait(false),
                    userId: user.Id,
                    groupId: group.Id,
                    name: optionName);

            await _mediator.Send(command).ConfigureAwait(false);


            return Response(ResponseStrings.AddedOptionToGroup(optionName, group.Name), ResponseType.User);
        }
    }
}