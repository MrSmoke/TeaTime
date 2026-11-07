namespace TeaTime.Slack.Resources;

using static StringHelpers;

internal static class ErrorStrings
{
    private const string HelpCommandText = "Type `/teatime help` for help";

    internal static string General() => "Oops, something went wrong :disappointed:";

    //Start
    internal static string StartRun_GroupInvalidName(string groupName) =>
        $"Failed to start round. *{EscapeMarkdown(groupName)}* is not a valid group. " +
        CommandStrings.AddGroup(groupName);

    internal static string StartRun_GroupNoOptions(string groupName) =>
        $"Cannot start round. *{EscapeMarkdown(groupName)}* has no available options. " + HelpCommandText;

    internal static string StartRun_RunAlreadyStarted() =>
        "Cannot start round. There is already an active TeaTime running in this room. " + CommandStrings.JoinRound;


    //Add option
    internal static string AddOption_GroupInvalidName(string groupName) =>
        $"Failed to add option. *{EscapeMarkdown(groupName)}* is not a valid group. " +
        CommandStrings.AddGroup(groupName);

    internal static string AddOption_DuplicateName(string optionName, string groupName) =>
        $"Failed to add option. *{EscapeMarkdown(optionName)}* already exists in *{EscapeMarkdown(groupName)}*";

    internal static string AddOption_BadArguments() =>
        "Failed to add option. Invalid command. " + CommandStrings.AddOption();


    //Remove option
    internal static string RemoveOption_GroupInvalidName(string groupName) =>
        $"Failed to remove option. *{EscapeMarkdown(groupName)}* is not a valid group.";

    internal static string RemoveOption_UnknownOption(string optionName) =>
        $"Failed to remove option. Unknown option *{EscapeMarkdown(optionName)}*";

    internal static string RemoveOption_BadArguments() =>
        "Failed to remove option. Invalid command. " + CommandStrings.RemoveOption();


    //Set option availability
    internal static string SetOptionAvailability_GroupInvalidName(string groupName) =>
        $"Failed to update option. *{EscapeMarkdown(groupName)}* is not a valid group.";

    internal static string SetOptionAvailability_UnknownOption(string optionName) =>
        $"Failed to update option. Unknown option *{EscapeMarkdown(optionName)}*";

    internal static string SetOptionAvailability_BadArguments(bool available)
    {
        if (available)
            return "Failed to update option. Invalid command. " + CommandStrings.OptionAvailable();

        return "Failed to update option. Invalid command. " + CommandStrings.OptionUnavailable();
    }


    //List options
    internal static string ListOptions_GroupInvalidName(string groupName) =>
        $"Failed to list options. *{EscapeMarkdown(groupName)}* is not a valid group.";

    internal static string ListOptions_GroupNoOptions(string groupName) =>
        $"Group *{EscapeMarkdown(groupName)}* has no options. " + CommandStrings.AddOption(groupName);

    internal static string ListOptions_BadArguments() =>
        "Failed to list options. Invalid command. " + CommandStrings.ListOptions();


    //Remove group
    internal static string RemoveGroup_GroupInvalidName(string groupName) =>
        $"Failed to remove group. Unknown group *{EscapeMarkdown(groupName)}*";

    internal static string RemoveGroup_BadArguments() =>
        "Failed to remove group. Invalid command. " + CommandStrings.RemoveGroup();


    //Join run
    internal static string JoinRun_RunNotStarted() => "Cannot join round. There is no TeaTime running. " + CommandStrings.StartRound;

    internal static string JoinRun_OptionUnknown(string optionName, string groupName) =>
        $"Failed to join round. Unknown option *{EscapeMarkdown(optionName)}*. " +
        CommandStrings.AddOption(groupName, optionName);

    internal static string JoinRun_OptionUnavailable(string optionName) =>
        $"Failed to join round. *{EscapeMarkdown(optionName)}* is currently unavailable";

    internal static string JoinRun_RunEnded() => "Failed to join round. Round has already ended";


    //Leave run
    internal static string LeaveRun_RunNotStarted() =>
        "Cannot leave round. There is no TeaTime running. " + CommandStrings.StartRound;

    internal static string LeaveRun_NotJoined() => "Cannot leave round. You haven't joined this round";

    internal static string LeaveRun_RunEnded() => "Cannot leave round. Round has already ended";


    //End run
    internal static string EndRun_RunNotStarted() =>
        "Cannot end round. There is no TeaTime running. " + CommandStrings.StartRound;

    internal static string EndRun_NoOrders() =>
        "Cannot end round. No one has joined yet :disappointed: " + CommandStrings.JoinRound;

    internal static string EndRun_NotJoined() =>
        "Cannot end round. You must join this round before ending it! " + CommandStrings.JoinRound;


    //Ill Make
    internal static string IllMake_RunNotStarted() =>
        "Cannot run illmake. There is no TeaTime running. " + CommandStrings.StartRound;

    //Misc
    internal static string OptionUnknown() => "Unknown option";

    internal static string CommandUnknown(string command) => $"Unknown command `{command}`. " + HelpCommandText;
    internal static string CommandFailed() => "Failed to run command";
    internal static string CommandBadArguments(string command) =>
        $"Invalid arguments for `{command}`. Use `/teatime help` to see the commands";
}
