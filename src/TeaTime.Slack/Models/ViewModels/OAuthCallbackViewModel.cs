namespace TeaTime.Slack.Models.ViewModels
{
    public class OAuthCallbackViewModel
    {
        public bool Success { get; }
        public string TeamName { get; set; }
        public string ErrorCode { get; private set; }

        public OAuthCallbackViewModel(bool success)
        {
            Success = success;
        }

        public static OAuthCallbackViewModel Ok(string teamName) => new OAuthCallbackViewModel(true)
        {
            TeamName = teamName
        };

        public static OAuthCallbackViewModel Error(string errorCode) => new OAuthCallbackViewModel(false)
        {
            ErrorCode = errorCode
        };
    }
}
