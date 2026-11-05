namespace TeaTime.Slack;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Common.Models.Data;
using Models.BlockKit;
using Resources;

internal static class BlockBuilder
{
    internal const string JoinOptionActionIdPrefix = "join_option_";

    internal const string LeaveRunActionId = "leave_run";

    // The number of actions (or buttons) per block.
    private const int ActionsPerBlock = 5;

    // We can have up to 50 blocks, but 20 for buttons is probably way too many anyway.
    private const int MaxActionBlocks = 20;

    internal static List<Block> BuildRunStarted(string text, IEnumerable<Option> options)
    {
        var contextBlock = new ContextBlock();
        contextBlock.Elements.Add(TextObject.Markdown(ResponseStrings.RunStartedHint()));

        List<Block> blocks =
        [
            new SectionBlock(TextObject.Markdown(text)),
            contextBlock
        ];

        // Show ActionsPerBlock buttons per block to avoid the "more" button.
        // Also only take MaxActionBlocks * ActionsPerBlock to limit the number of rows.
        var outputOptions = options
            .Take(ActionsPerBlock * MaxActionBlocks)
            .Chunk(ActionsPerBlock);

        foreach (var chunk in outputOptions)
        {
            var actions = new ActionsBlock();
            actions.Elements.AddRange(chunk.Select(CreateJoinButton));
            blocks.Add(actions);
        }

        // todo: add this back when we have ill make and other buttons
        // var controls = new ActionsBlock();
        // controls.Elements.Add(CreateLeaveButton(runId));

        //blocks.Add(new DividerBlock());
        //blocks.Add(controls);

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

    private static ButtonElement CreateLeaveButton(long runId)
    {
        // The run id lets a click on an old round's message be rejected instead of leaving the current round
        return new ButtonElement
        {
            Text = TextObject.PlainText("Leave round"),
            ActionId = LeaveRunActionId,
            Value = runId.ToString(CultureInfo.InvariantCulture)
        };
    }
}
