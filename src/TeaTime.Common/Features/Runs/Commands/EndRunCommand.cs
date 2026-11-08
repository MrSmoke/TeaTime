namespace TeaTime.Common.Features.Runs.Commands;

using Abstractions;

/// <summary>
/// The command to end a run
/// </summary>
public record EndRunCommand(long RunId, long CurrentUserId) : BaseCommand, IUserCommand;
