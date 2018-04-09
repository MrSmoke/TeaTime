namespace TeaTime.Slack.Exceptions
{
    using Common.Exceptions;

    public class SlackTeaTimeException : TeaTimeException
    {
        public SlackTeaTimeException(string message) : base(message)
        {
        }
    }
}
