namespace TeaTime.Slack.Services
{
    using System;
    using Configuration;
    using Microsoft.Extensions.Options;
    using Models.Requests;

    public class SlackMessageVerifier : ISlackMessageVerifier
    {
        private readonly IOptionsMonitor<SlackOptions> _optionsMonitor;

        public SlackMessageVerifier(IOptionsMonitor<SlackOptions> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        public bool IsValid(IVerifiableRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.Token is null)
                return false;

            return request.Token.Equals(_optionsMonitor.CurrentValue.VerificationToken, StringComparison.Ordinal);
        }
    }
}
