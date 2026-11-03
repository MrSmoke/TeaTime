namespace TeaTime.Slack.Commands;

using CommandRouter.Attributes;
using CommandRouter.Results;
using Services;

public class HelpCommand(ISlackService slackService) : BaseCommand(slackService)
{
    [Command("help")]
    public ICommandResult Help()
    {
        const string helpMessage =
            "*Commands*\n" +
            "`/teatime start` - Start a new round of tea\n" +
            "`/teatime start {group}` - Start a new round for another group, like `/teatime start coffee`\n" +
            "`/teatime join {option}` - Join the current round\n" +
            "`/teatime leave` - Leave the current round\n" +
            "`/teatime end` - End a round\n" +
            "`/teatime illmake` - Volunteer to make the round\n" +
            "`/teatime groups add {name}` - Add a group\n" +
            "`/teatime groups remove {name}` - Remove a group\n" +
            "`/teatime options add {group} {name}` - Add a new option for a group\n" +
            "`/teatime options remove {group} {name}` - Remove an option from a group\n" +
            "\n" +
            "Use double quotes for names with spaces, for example `/teatime options add tea \"earl grey\"`\n" +
            "\n\n" +
            "*Examples*\n" +
            "To start a new round of tea:\n" +
            "`/teatime start`";

        return StringResult(helpMessage);
    }
}
