namespace TeaTime.Slack.Resources;

using System.Collections.Generic;
using System.Linq;
using Common.Models.Data;

internal static class ResponseStrings
{
    internal static string OptionAddedToGroup(string optionName, string groupName) =>
        $"Added option *{optionName}* to group *{groupName}*";

    internal static string OptionRemoved(string optionName, string groupName) =>
        $"Removed option *{optionName}* from group *{groupName}*";

    internal static string OptionAvailabilityChanged(string optionName, bool available)
    {
        if (available)
            return $"*{optionName}* is now available";

        return $"*{optionName}* is now unavailable";
    }

    internal static string RunStarted(string slackUserId, string groupName) =>
        $"{StringHelpers.SlackUserId(slackUserId)} wants *{StringHelpers.EscapeMarkdown(groupName)}*!";

    internal static string RunStartedHint() => "Pick an option below to join, or type `/teatime join {option}`";

    internal static string RunUserJoined(string slackUserId) =>
        $"{StringHelpers.SlackUserId(slackUserId)} has joined this round!";

    internal static string RunUserLeft(string slackUserId) =>
        $"{StringHelpers.SlackUserId(slackUserId)} has left this round";

    internal static string RunUserOrderChanged(string from, string to) =>
        $"You have changed your order: {from} -> {to}";

    internal static string GroupAdded(string groupName) => $"Group *{groupName}* created!";

    internal static string GroupRemoved(string groupName) => $"Group *{groupName}* has been removed.";

    internal static string IllMake(string slackUserId) =>
        $"{StringHelpers.SlackUserId(slackUserId)} has volunteered to make this round!";

    internal static string OptionList(string groupName, IReadOnlyCollection<Option> options)
    {
        var available = options.Where(o => o.Available).Select(o => $"- {o.Name}");
        var unavailable = options.Where(o => !o.Available).Select(o => $"- {o.Name}").ToList();

        var text = $"Options for *{groupName}*\n" + string.Join('\n', available);

        if (unavailable.Count == 0)
            return text;

        return text + "\n\n*Unavailable*\n" + string.Join('\n', unavailable);
    }
}
