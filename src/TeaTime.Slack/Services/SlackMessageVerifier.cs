namespace TeaTime.Slack.Services
{
    using Models.Requests;

    public class SlackMessageVerifier : ISlackMessageVerifier
    {
        private readonly string _verificationToken;

        public SlackMessageVerifier(SlackOptions options)
        {
            _verificationToken = options.VerificationToken;
        }

        public bool IsValid(IVerifiableRequest request)
        {
            return request.Token.Equals(_verificationToken);
        }
    }
}
