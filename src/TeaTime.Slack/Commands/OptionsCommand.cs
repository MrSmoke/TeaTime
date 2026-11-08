namespace TeaTime.Slack.Commands;

using System;
using System.Linq;
using System.Threading.Tasks;
using CommandRouter.Attributes;
using CommandRouter.Results;
using Common.Abstractions;
using Common.Exceptions;
using Common.Features.Options.Commands;
using Common.Features.RoomItemGroups.Queries;
using MediatR;
using Models.SlashCommands;
using Resources;
using Services;

[CommandPrefix("options")]
public class OptionsCommand(
    ISlackService slackService,
    IMediator mediator,
    IIdGenerator<long> idGenerator)
    : BaseCommand(slackService)
{
    [Command("add")]
    public async Task<ICommandResult> AddOption(string? groupName, string? optionName)
    {
        if (string.IsNullOrWhiteSpace(groupName) || string.IsNullOrWhiteSpace(optionName))
            return Response(ErrorStrings.AddOption_BadArguments(), ResponseType.User);

        var context = await GetContextAsync();

        var group = await mediator.Send(new GetRoomItemGroupByNameQuery(
            RoomId: context.Room.Id,
            UserId: context.User.Id,
            Name: groupName));

        if (group == null)
            return Response(ErrorStrings.AddOption_GroupInvalidName(groupName), ResponseType.User);

        var command = new CreateOptionCommand(
                Id: await idGenerator.GenerateAsync(),
                CurrentUserId: context.User.Id,
                GroupId: group.Id,
                Name: optionName);

        try
        {
            await mediator.Send(command);
        }
        catch (OptionException e) when (e.Reason == OptionException.OptionExceptionReason.DuplicateName)
        {
            return Response(ErrorStrings.AddOption_DuplicateName(optionName, group.Name), ResponseType.User);
        }

        return Response(ResponseStrings.OptionAddedToGroup(optionName, group.Name), ResponseType.User);
    }

    [Command("list")]
    public async Task<ICommandResult> ListOptions(string? groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
            return Response(ErrorStrings.ListOptions_BadArguments(), ResponseType.User);

        var context = await GetContextAsync();

        var group = await mediator.Send(new GetRoomItemGroupByNameQuery(
            RoomId: context.Room.Id,
            UserId: context.User.Id,
            Name: groupName));

        if (group == null)
            return Response(ErrorStrings.ListOptions_GroupInvalidName(groupName), ResponseType.User);

        if (group.Options.Count == 0)
            return Response(ErrorStrings.ListOptions_GroupNoOptions(group.Name), ResponseType.User);

        return Response(ResponseStrings.OptionList(group.Name, group.Options), ResponseType.User);
    }

    [Command("remove")]
    public async Task<ICommandResult> RemoveOption(string? groupName, string? optionName)
    {
        if (string.IsNullOrWhiteSpace(groupName) || string.IsNullOrWhiteSpace(optionName))
            return Response(ErrorStrings.RemoveOption_BadArguments(), ResponseType.User);

        var context = await GetContextAsync();

        var group = await mediator.Send(new GetRoomItemGroupByNameQuery(
            RoomId: context.Room.Id,
            UserId: context.User.Id,
            Name: groupName));

        if (group == null)
            return Response(ErrorStrings.RemoveOption_GroupInvalidName(groupName), ResponseType.User);

        var option = group.Options.FirstOrDefault(o => o.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase));
        if (option == null)
            return Response(ErrorStrings.RemoveOption_UnknownOption(optionName), ResponseType.User);

        var command = new DeleteOptionCommand(option.Id, context.User.Id);

        await mediator.Send(command);

        return Response(ResponseStrings.OptionRemoved(option.Name, group.Name), ResponseType.User);
    }

    [Command("available")]
    public Task<ICommandResult> MarkOptionAvailable(string? groupName, string? optionName)
    {
        return SetOptionAvailabilityAsync(groupName, optionName, available: true);
    }

    [Command("unavailable")]
    public Task<ICommandResult> MarkOptionUnavailable(string? groupName, string? optionName)
    {
        return SetOptionAvailabilityAsync(groupName, optionName, available: false);
    }

    private async Task<ICommandResult> SetOptionAvailabilityAsync(string? groupName, string? optionName,
        bool available)
    {
        if (string.IsNullOrWhiteSpace(groupName) || string.IsNullOrWhiteSpace(optionName))
            return Response(ErrorStrings.SetOptionAvailability_BadArguments(available), ResponseType.User);

        var context = await GetContextAsync();

        var group = await mediator.Send(new GetRoomItemGroupByNameQuery(
            RoomId: context.Room.Id,
            UserId: context.User.Id,
            Name: groupName));

        if (group == null)
            return Response(ErrorStrings.SetOptionAvailability_GroupInvalidName(groupName), ResponseType.User);

        var option = group.Options.FirstOrDefault(o => o.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase));
        if (option == null)
            return Response(ErrorStrings.SetOptionAvailability_UnknownOption(optionName), ResponseType.User);

        if (option.Available != available)
        {
            var command = new SetOptionAvailabilityCommand(option.Id, context.User.Id, available);

            await mediator.Send(command);
        }

        return Response(ResponseStrings.OptionAvailabilityChanged(option.Name, available), ResponseType.User);
    }
}
