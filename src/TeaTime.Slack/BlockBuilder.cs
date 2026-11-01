namespace TeaTime.Slack
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Common.Models.Data;
    using Models.BlockKit;

    internal static class BlockBuilder
    {
        internal const string JoinOptionActionIdPrefix = "join_option_";

        internal static List<Block> BuildRunStarted(string text, IEnumerable<Option> options)
        {
            var blocks = new List<Block>
            {
                new SectionBlock(TextObject.Markdown(text)),
                new SectionBlock(TextObject.PlainText("Select an option to join this round!"))
            };

            foreach (var chunk in options.Chunk(ActionsBlock.MaxElements))
            {
                var actions = new ActionsBlock();
                actions.Elements.AddRange(chunk.Select(CreateJoinButton));
                blocks.Add(actions);
            }

            return blocks;
        }

        private static ButtonElement CreateJoinButton(Option option)
        {
            var id = option.Id.ToString(CultureInfo.InvariantCulture);

            return new ButtonElement
            {
                Text = TextObject.PlainText(option.Name),
                ActionId = JoinOptionActionIdPrefix + id,
                Value = id
            };
        }
    }
}
