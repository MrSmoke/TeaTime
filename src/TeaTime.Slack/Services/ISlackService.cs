namespace TeaTime.Slack.Services
{
    using System.Threading.Tasks;
    using Common.Models.Data;

    public interface ISlackService
    {
        Task<User> GetOrCreateUser(string userId, string name);
        Task<Room> GetOrCreateRoom(string channelId, string channelName, long userId);
    }
}