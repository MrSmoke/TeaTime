namespace TeaTime.Slack.Client
{
    using System.Threading.Tasks;
    using Models.Responses;

    public interface ISlackApiClient
    {
        Task PostResponseAsync(string callbackUrl, SlashCommandResponse response);
    }
}