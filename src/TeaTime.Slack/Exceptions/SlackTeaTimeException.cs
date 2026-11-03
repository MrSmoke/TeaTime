namespace TeaTime.Slack.Exceptions;

using Common.Exceptions;

public class SlackTeaTimeException(string message) : TeaTimeException(message);
