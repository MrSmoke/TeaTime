namespace TeaTime.Slack.Resources;

using System;
using System.Linq;

internal static class StringHelpers
{
    internal static string SlackUserId(string id) => $"<@{id}>";

    /// <summary>
    /// Escapes the control characters Slack mrkdwn requires, so user input can't form links or mentions
    /// </summary>
    internal static string EscapeMarkdown(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);
    }

    /// <summary>
    /// Wraps a value containing whitespace in quotes, so it's a single argument in a suggested command
    /// </summary>
    internal static string QuoteArgument(string value) => value.Any(char.IsWhiteSpace) ? $"\"{value}\"" : value;
}
