namespace TeaTime.Slack.Services
{
    using System.Threading.Tasks;
    using Common.Models.Data;
    using Models.OAuth;
    using Models.Requests;
    using Models.Requests.InteractiveMessages;

    public interface ISlackService
    {
        Task<User> GetOrCreateUser(string userId, string name);
        Task<Room> GetOrCreateRoom(string channelId, string channelName, long userId);

        Task JoinRunAsync(SlashCommand slashCommand, string optionName);
        Task JoinRunAsync(MessageRequestPayload requestPayload);

        Task InstallAsync(OAuthTokenResponse response);
    }
}
