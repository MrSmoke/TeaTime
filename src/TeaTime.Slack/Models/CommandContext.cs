namespace TeaTime.Slack.Models
{
    using Common.Models.Data;
    using Requests;

    public class CommandContext
    {
        public SlashCommand Command { get; set; }
        public User User { get; set; }
        public Room Room { get; set; }
    }
}
