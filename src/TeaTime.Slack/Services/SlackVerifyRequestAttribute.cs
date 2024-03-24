namespace TeaTime.Slack.Services;

using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
public class SlackVerifyRequestAttribute : Attribute;
