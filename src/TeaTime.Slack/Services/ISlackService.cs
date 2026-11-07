namespace TeaTime.Slack.Services;

using System.Threading;
using System.Threading.Tasks;
using Common.Models.Data;
using Models.Interactivity;
using Models.OAuth;
using Models.SlashCommands;

public interface ISlackService
{
    Task InstallAsync(OAuthTokenResponse response, CancellationToken cancellationToken = default);

    Task<User> GetOrCreateUserAsync(string userId, string? name, CancellationToken cancellationToken = default);

    Task<Room> GetOrCreateRoomAsync(string channelId, string? channelName, long userId,
        CancellationToken cancellationToken = default);

    Task JoinRunAsync(SlashCommand slashCommand, string optionName, CancellationToken cancellationToken = default);
    Task LeaveRunAsync(SlashCommand slashCommand, CancellationToken cancellationToken = default);

    Task HandleBlockActionsAsync(BlockActionsPayload requestPayload, CancellationToken cancellationToken = default);
}
