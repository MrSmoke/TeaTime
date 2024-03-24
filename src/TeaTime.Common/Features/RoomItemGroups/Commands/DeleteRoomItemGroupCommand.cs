namespace TeaTime.Common.Features.RoomItemGroups.Commands;

using Abstractions;

public record DeleteRoomItemGroupCommand(long GroupId, long UserId) : BaseCommand, IUserCommand;
