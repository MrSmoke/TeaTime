namespace TeaTime.Common.Features.RoomItemGroups.EventHandlers;

using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Commands;
using MediatR;
using Options.Commands;
using Rooms.Events;

public class RoomCreatedEventHandler(
    IMediator mediator,
    IIdGenerator<long> idGenerator)
    : INotificationHandler<RoomCreatedEvent>
{
    private static readonly string[] DefaultItemNames = ["Earl Grey", "English Breakfast"];

    public async Task Handle(RoomCreatedEvent notification, CancellationToken cancellationToken)
    {
        if (!notification.CreateDefaultItemGroup)
            return;

        var command = new CreateRoomItemGroupCommand(
            Id: await idGenerator.GenerateAsync(CancellationToken.None),
            RoomId: notification.Id,
            UserId: notification.CreatedBy,
            Name: "tea");

        await mediator.Send(command, CancellationToken.None);

        //create some options
        foreach (var name in DefaultItemNames)
        {
            var optionCommand = new CreateOptionCommand(
                Id: await idGenerator.GenerateAsync(CancellationToken.None),
                UserId: notification.CreatedBy,
                GroupId: command.Id,
                Name: name);

            await mediator.Send(optionCommand, CancellationToken.None);
        }
    }
}
