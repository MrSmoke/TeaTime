namespace TeaTime.Slack.Commands
{
    using CommandRouter.Attributes;
    using CommandRouter.Results;
    using Services;

    public class HelpCommand : BaseCommand
    {
        public HelpCommand(ISlackService slackService) : base(slackService)
        {
        }

        [Command("help")]
        public ICommandResult Help()
        {
            const string helpMessage =
                "*Commands*\n" +
                "`/teatime {group}` - Start a new round\n" +
                "`/teatime end` - End a round\n" +
                "`/teatime illmake` - Volunteer to make the round\n" +
                "`/teatime groups add {name}` - Add a group\n" +
                "`/teatime groups remove {name}` - Remove a group\n" +
                "`/teatime options add {group} {name}` - Add a new option for a group\n" +
                "`/teatime options remove {group} {name}` - Remove an option from a group\n" +
                "\n\n" +
                "*Examples*\n" +
                "To start a new round:\n" +
                "`/teatime tea`";

            return StringResult(helpMessage);
        }
    }
}