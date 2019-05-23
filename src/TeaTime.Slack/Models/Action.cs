namespace TeaTime.Slack.Models
{
    public abstract class Action
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }

        public abstract string Type { get; }
    }

    public class Button : Action
    {
        public override string Type => "button";

        public string Style { get; set; } = "default";
    }
}