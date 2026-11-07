namespace TeaTime.Slack.Resources;

using System.Collections.Generic;
using System.Linq;
using Common.Models.Data;
using static StringHelpers;

internal static class ResponseStrings
{
    internal static string OptionAddedToGroup(string optionName, string groupName) =>
        $"Added option *{EscapeMarkdown(optionName)}* to group *{EscapeMarkdown(groupName)}*";

    internal static string OptionRemoved(string optionName, string groupName) =>
        $"Removed option *{EscapeMarkdown(optionName)}* from group *{EscapeMarkdown(groupName)}*";

    internal static string OptionAvailabilityChanged(string optionName, bool available)
    {
        if (available)
            return $"*{EscapeMarkdown(optionName)}* is now available";

        return $"*{EscapeMarkdown(optionName)}* is now unavailable";
    }

    internal static string RunStarted(string slackUserId, string groupName) =>
        $"{SlackUserId(slackUserId)} wants *{EscapeMarkdown(groupName)}*!";

    internal static string RunStartedHint() => "Pick an option below to join, or type `/teatime join {option}`";

    internal static string RunUserJoined(string slackUserId) =>
        $"{SlackUserId(slackUserId)} has joined this round!";

    internal static string RunUserLeft(string slackUserId) =>
        $"{SlackUserId(slackUserId)} has left this round";

    internal static string RunUserOrderChanged(string from, string to) =>
        $"You have changed your order: {EscapeMarkdown(from)} -> {EscapeMarkdown(to)}";

    internal static string GroupAdded(string groupName) => $"Group *{EscapeMarkdown(groupName)}* created!";

    internal static string GroupRemoved(string groupName) => $"Group *{EscapeMarkdown(groupName)}* has been removed.";

    internal static string IllMake(string slackUserId) =>
        $"{SlackUserId(slackUserId)} has volunteered to make this round!";

    internal static string OptionList(string groupName, IReadOnlyCollection<OptionEntity> options)
    {
        var available = options.Where(o => o.Available).Select(o => $"- {EscapeMarkdown(o.Name)}").ToList();
        var unavailable = options.Where(o => !o.Available).Select(o => $"- {EscapeMarkdown(o.Name)}").ToList();

        var text = "*Available*\n";

        if (available.Count == 0)
            text += "No options are currently available";
        else
            text += string.Join('\n', available);

        if (unavailable.Count == 0)
            return text;

        return text + "\n\n*Unavailable*\n" + string.Join('\n', unavailable);
    }
}
