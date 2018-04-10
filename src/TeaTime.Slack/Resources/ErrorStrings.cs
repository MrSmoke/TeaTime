namespace TeaTime.Slack.Resources
{
    internal static class ErrorStrings
    {
        private const string HelpCommandText = "Type `/teatime help` for help";


        internal static string General() => "Oops, something went wrong :disappointed:";

        internal static string StartRun_GroupInvalidName(string groupName) =>
            $"Failed to start round. *{groupName}* is not a valid group. " + CommandStrings.AddGroup(groupName);

        internal static string StartRun_GroupNoOptions(string groupName) =>
            $"Cannot start round. *{groupName}* has no options. " + CommandStrings.AddOption(groupName);

        internal static string StartRun_RunAlreadyStarted() =>
            "Cannot start round. There is already an active TeaTime running in this room. " + CommandStrings.JoinRound;

        internal static string AddOption_GroupInvalidName(string groupName) =>
            $"Failed to add option. *{groupName}* is not a valid group. " + CommandStrings.AddGroup(groupName);


        internal static string JoinRun_RunNotStarted() => "Cannot join round. There is no TeaTime running. " + CommandStrings.StartRound;

        internal static string JoinRun_OptionUnknown(string optionName, string groupName) =>
            $"Failed to join round. Unknown option *{optionName}*. " + CommandStrings.AddOption(groupName, optionName);

        internal static string JoinRun_RunEnded() => "Failed to join round. Round has already ended";

        internal static string EndRun_RunNotStarted() =>
            "Cannot end round. There is no TeaTime running. " + CommandStrings.StartRound;

        internal static string EndRun_NoOrders() =>
            "Cannot end round. No one has joined yet :disappointed: " + CommandStrings.JoinRound;

        internal static string EndRun_NotJoined() =>
            "Cannot end round. You must join this round before ending it! " + CommandStrings.JoinRound;

        internal static string OptionUnknown() => "Unknown option";

        internal static string CommandUnknown(string command) => $"Unknown command `{command}`";
        internal static string CommandFailed() => "Failed to run command";
    }
}