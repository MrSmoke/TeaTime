namespace TeaTime.Slack.Models
{
    public class Button : Action
    {
        public override string Type => "button";

        public string Style { get; set; } = "default";
    }
}