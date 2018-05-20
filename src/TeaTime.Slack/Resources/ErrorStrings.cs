namespace TeaTime.Slack.Resources
{
    internal static class ErrorStrings
    {
        private const string HelpCommandText = "Type `/teatime help` for help";

        internal static string General() => "Oops, something went wrong :disappointed:";

        //Start
        internal static string StartRun_GroupInvalidName(string groupName) =>
            $"Failed to start round. *{groupName}* is not a valid group. " + CommandStrings.AddGroup(groupName);

        internal static string StartRun_GroupNoOptions(string groupName) =>
            $"Cannot start round. *{groupName}* has no options. " + CommandStrings.AddOption(groupName);

        internal static string StartRun_RunAlreadyStarted() =>
            "Cannot start round. There is already an active TeaTime running in this room. " + CommandStrings.JoinRound;


        //Add option
        internal static string AddOption_GroupInvalidName(string groupName) =>
            $"Failed to add option. *{groupName}* is not a valid group. " + CommandStrings.AddGroup(groupName);

        internal static string AddOption_BadArguments() =>
            "Failed to add option. Invalid command. " + CommandStrings.AddOption();


        //Remove option
        internal static string RemoveOption_GroupInvalidName(string groupName) =>
            $"Failed to remove option. *{groupName}* is not a valid group.";

        internal static string RemoveOption_UnknownOption(string optionName) =>
            $"Failed to remove option. Unknown option *{optionName}*";

        internal static string RemoveOption_BadArguments() =>
            "Failed to remove option. Invalid command. " + CommandStrings.AddOption();


        //Remove group
        internal static string RemoveGroup_GroupInvalidName(string groupName) =>
            $"Failed to remove group. Unknown group *{groupName}*";

        internal static string RemoveGroup_BadArguments() =>
            "Failed to remove group. Invalid command. " + CommandStrings.AddOption();


        //Join run
        internal static string JoinRun_RunNotStarted() => "Cannot join round. There is no TeaTime running. " + CommandStrings.StartRound;

        internal static string JoinRun_OptionUnknown(string optionName, string groupName) =>
            $"Failed to join round. Unknown option *{optionName}*. " + CommandStrings.AddOption(groupName, optionName);

        internal static string JoinRun_RunEnded() => "Failed to join round. Round has already ended";


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

        internal static string CommandUnknown(string command) => $"Unknown command `{command}`";
        internal static string CommandFailed() => "Failed to run command";
    }
}