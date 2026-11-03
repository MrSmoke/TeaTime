namespace TeaTime.Slack.Exceptions;

using System;
using Common.Exceptions;

public class SlackTeaTimeException(string message, Exception? innerException = null)
    : TeaTimeException(message, innerException);
