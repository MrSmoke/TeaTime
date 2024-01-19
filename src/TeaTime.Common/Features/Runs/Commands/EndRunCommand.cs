namespace TeaTime.Common.Features.Runs.Commands;

using System.Collections.Generic;
using Abstractions;
using Models.Domain;

/// <summary>
/// The command to end a run
/// </summary>
public record EndRunCommand(long RunId, long RoomId, long UserId, IEnumerable<OrderModel> Orders)
    : BaseCommand, IUserCommand;
