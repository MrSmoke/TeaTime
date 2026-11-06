namespace TeaTime.Slack.Tests;

using System;
using System.Linq;
using System.Text.Json.Nodes;
using Common.Models.Data;
using Models.BlockKit;
using Models.Interactivity;
using Models.SlashCommands;
using Xunit;

public class BlockKitTests
{
    // Buttons are split into rows of 5, keep the order the options were given in and carry the run id
    [Fact]
    public void BuildRunStarted_ChunksButtonsIntoActionsBlocks()
    {
        var options = Enumerable.Range(1, 12).Select(CreateOption).ToList();

        var blocks = BlockBuilder.BuildRunStarted("<@U1> started a run", runId: 9, options);

        var actions = blocks.OfType<ActionsBlock>().ToList();
        var rowSizes = actions.Select(a => a.Elements.Count);
        Assert.Equal([5, 5, 2], rowSizes);

        var values = actions.SelectMany(a => a.Elements).Select(e => e.Value);
        var expected = options.Select(o => "9:" + o.Id.ToString());
        Assert.Equal(expected, values);
    }

    // Only the first 100 options get buttons, as 20 full rows
    [Fact]
    public void BuildRunStarted_CapsNumberOfButtons()
    {
        var options = Enumerable.Range(1, 120).Select(CreateOption).ToList();

        var blocks = BlockBuilder.BuildRunStarted("<@U1> started a run", runId: 9, options);

        var actions = blocks.OfType<ActionsBlock>().ToList();
        Assert.Equal(20, actions.Count);
        Assert.All(actions, a => Assert.Equal(5, a.Elements.Count));

        var last = actions[^1].Elements[^1];
        Assert.Equal("9:100", last.Value);
        Assert.Equal(BlockBuilder.JoinOptionActionIdPrefix + "100", last.ActionId);
    }

    // A join value is exactly two numbers separated by a colon
    [Theory]
    [InlineData("7:1", true, 7, 1)]
    [InlineData("1", false, 0, 0)]
    [InlineData("a:b", false, 0, 0)]
    [InlineData("1:2:3", false, 0, 0)]
    [InlineData("", false, 0, 0)]
    [InlineData(null, false, 0, 0)]
    public void TryParseJoinValue_ParsesOnlyRunAndOptionIds(string? value, bool expected, long runId, long optionId)
    {
        var parsed = BlockBuilder.TryParseJoinValue(value, out var actualRunId, out var actualOptionId);

        Assert.Equal(expected, parsed);

        if (expected)
        {
            Assert.Equal(runId, actualRunId);
            Assert.Equal(optionId, actualOptionId);
        }
    }

    [Fact]
    public void Response_SerialisesBlocksAndOmitsNulls()
    {
        var button = new ButtonElement
        {
            Text = TextObject.PlainText("Tea"),
            ActionId = "join_option_7",
            Value = "7"
        };

        var actionsBlock = new ActionsBlock();
        actionsBlock.Elements.Add(button);

        var response = new SlashCommandResponse
        {
            Text = "fallback",
            Type = ResponseType.Channel,
            Blocks = [new SectionBlock(TextObject.Markdown("hi")), actionsBlock]
        };

        var json = JsonNode.Parse(SlackJsonSerializer.Serialize(response))!.AsObject();

        Assert.Equal("fallback", (string?)json["text"]);
        Assert.Equal("in_channel", (string?)json["response_type"]);
        Assert.False(json.ContainsKey("delete_original"));
        Assert.False((bool)json["replace_original"]!);
        Assert.False(json.ContainsKey("attachments"));

        var blocks = Assert.IsType<JsonArray>(json["blocks"]);
        Assert.Equal(2, blocks.Count);

        var section = Assert.IsType<JsonObject>(blocks[0]);
        var sectionText = Assert.IsType<JsonObject>(section["text"]);
        Assert.Equal("section", (string?)section["type"]);
        Assert.Equal("mrkdwn", (string?)sectionText["type"]);
        Assert.False(sectionText.ContainsKey("emoji"));

        var actions = Assert.IsType<JsonObject>(blocks[1]);
        var buttonJson = Assert.IsType<JsonObject>(actions["elements"]?[0]);
        Assert.Equal("actions", (string?)actions["type"]);
        Assert.Equal("button", (string?)buttonJson["type"]);
        Assert.Equal("join_option_7", (string?)buttonJson["action_id"]);
        Assert.Equal("7", (string?)buttonJson["value"]);
        Assert.Equal("plain_text", (string?)buttonJson["text"]?["type"]);
    }

    [Fact]
    public void BlockActionsPayload_Deserialises()
    {
        const string json = """
            {
              "type": "block_actions",
              "user": { "id": "U1", "username": "steve", "team_id": "T1" },
              "api_app_id": "A1",
              "token": "deprecated",
              "container": { "type": "message", "message_ts": "1.2", "channel_id": "C1", "is_ephemeral": false },
              "trigger_id": "123.456",
              "team": { "id": "T1", "domain": "example" },
              "enterprise": null,
              "is_enterprise_install": false,
              "channel": { "id": "C1", "name": "general" },
              "message": { "type": "message", "ts": "1.2" },
              "state": { "values": {} },
              "response_url": "https://hooks.slack.com/actions/T1/1/abc",
              "actions": [
                {
                  "action_id": "join_option_42",
                  "block_id": "b1",
                  "text": { "type": "plain_text", "text": "Tea", "emoji": true },
                  "value": "42",
                  "type": "button",
                  "action_ts": "1.3"
                }
              ]
            }
            """;

        var payload = SlackJsonSerializer.Deserialize<BlockActionsPayload>(json)!;

        Assert.Equal(BlockActionsPayload.PayloadType, payload.Type);
        Assert.Equal("U1", payload.User.Id);
        Assert.Equal("steve", payload.User.Username);
        Assert.Equal("C1", payload.Channel!.Id);
        Assert.Equal("https://hooks.slack.com/actions/T1/1/abc", payload.ResponseUrl);

        var action = Assert.Single(payload.Actions);
        Assert.Equal("join_option_42", action.ActionId);
        Assert.Equal("42", action.Value);
    }

    private static Option CreateOption(int id) => new()
    {
        Id = id,
        Name = "option" + id,
        Available = true,
        CreatedBy = 1,
        CreatedDate = DateTimeOffset.UtcNow,
        GroupId = 1
    };
}
