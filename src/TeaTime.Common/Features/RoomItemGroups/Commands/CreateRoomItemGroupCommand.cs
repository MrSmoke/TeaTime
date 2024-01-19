namespace TeaTime.Common.Features.RoomItemGroups.Commands;

using Abstractions;

public record CreateRoomItemGroupCommand(long Id, long RoomId, string Name, long UserId) : IUserCommand;
