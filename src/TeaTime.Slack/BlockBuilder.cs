namespace TeaTime.Slack;

using System.Collections.Generic;
using System.Linq;
using Common.Models.Data;
using Models.BlockKit;
using Resources;

internal static class BlockBuilder
{
    internal const string JoinOptionActionIdPrefix = "join_option_";

    internal const string LeaveRunActionId = "leave_run";

    // Separates the run id and option id in a join button's value
    private const char JoinValueSeparator = ':';

    // The number of actions (or buttons) per block.
    private const int ActionsPerBlock = 5;

    // We can have up to 50 blocks, but 20 for buttons is probably way too many anyway.
    private const int MaxActionBlocks = 20;

    internal static List<Block> BuildRunStarted(string text, long runId, IEnumerable<OptionEntity> options)
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
            var buttons = chunk.Select(o => CreateJoinButton(runId, o));
            actions.Elements.AddRange(buttons);
            blocks.Add(actions);
        }

        // todo: add this back when we have ill make and other buttons
        // var controls = new ActionsBlock();
        // controls.Elements.Add(CreateLeaveButton(runId));

        //blocks.Add(new DividerBlock());
        //blocks.Add(controls);

        return blocks;
    }

    internal static bool TryParseJoinValue(string? value, out long runId, out long optionId)
    {
        runId = 0;
        optionId = 0;

        var parts = value?.Split(JoinValueSeparator);
        if (parts is not { Length: 2 })
            return false;

        return long.TryParse(parts[0], out runId) && long.TryParse(parts[1], out optionId);
    }

    private static ButtonElement CreateJoinButton(long runId, OptionEntity option)
    {
        var id = option.Id.ToString();

        // The run id ties a click to this round, so an old round's message can't change the current round
        var value = $"{runId}{JoinValueSeparator}{id}";

        return new ButtonElement
        {
            Text = TextObject.PlainText(option.Name),
            ActionId = JoinOptionActionIdPrefix + id,
            Value = value
        };
    }

    private static ButtonElement CreateLeaveButton(long runId)
    {
        // The run id lets a click on an old round's message be rejected instead of leaving the current round
        return new ButtonElement
        {
            Text = TextObject.PlainText("Leave round"),
            ActionId = LeaveRunActionId,
            Value = runId.ToString()
        };
    }
}
