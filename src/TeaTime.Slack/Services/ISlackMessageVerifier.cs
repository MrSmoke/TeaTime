namespace TeaTime.Slack.Services
{
    using Models.Requests;

    public interface ISlackMessageVerifier
    {
        bool IsValid(IVerifiableRequest request);
    }
}