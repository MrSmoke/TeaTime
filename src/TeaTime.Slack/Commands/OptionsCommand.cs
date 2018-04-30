namespace TeaTime.Slack.Commands
{
    using System.Linq;
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
            if (string.IsNullOrWhiteSpace(groupName) || string.IsNullOrWhiteSpace(optionName))
                return Response(ErrorStrings.AddOption_BadArguments(), ResponseType.User);

            var context = await GetContextAsync().ConfigureAwait(false);

            var group = await _mediator.Send(new GetRoomItemGroupByNameQuery(
                roomId: context.Room.Id,
                userId: context.User.Id,
                name: groupName)).ConfigureAwait(false);

            if (group == null)
                return Response(ErrorStrings.AddOption_GroupInvalidName(groupName), ResponseType.User);

            var command =
                new CreateOptionCommand(
                    id: await _idGenerator.GenerateAsync().ConfigureAwait(false),
                    userId: context.User.Id,
                    groupId: group.Id,
                    name: optionName);

            await _mediator.Send(command).ConfigureAwait(false);

            return Response(ResponseStrings.OptionAddedToGroup(optionName, group.Name), ResponseType.User);
        }

        [Command("remove")]
        public async Task<ICommandResult> RemoveOption(string groupName, string optionName)
        {
            if (string.IsNullOrWhiteSpace(groupName) || string.IsNullOrWhiteSpace(optionName))
                return Response(ErrorStrings.RemoveOption_BadArguments(), ResponseType.User);

            var context = await GetContextAsync().ConfigureAwait(false);

            var group = await _mediator.Send(new GetRoomItemGroupByNameQuery(
                roomId: context.Room.Id,
                userId: context.User.Id,
                name: groupName)).ConfigureAwait(false);

            if (group == null)
                return Response(ErrorStrings.RemoveOption_GroupInvalidName(groupName), ResponseType.User);

            var option = group.Options.FirstOrDefault(o => o.Name.Equals(optionName));
            if (option == null)
                return Response(ErrorStrings.RemoveOption_UnknownOption(optionName), ResponseType.User);

            var command = new DeleteOptionCommand(option.Id, context.User.Id);

            await _mediator.Send(command).ConfigureAwait(false);

            return Response(ResponseStrings.OptionRemoved(optionName, groupName), ResponseType.User);
        }
    }
}