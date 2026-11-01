namespace TeaTime.Slack.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Common.Models.Data;
using Models.BlockKit;
using Models.Interactivity;
using Models.SlashCommands;
using Xunit;

public class BlockKitTests
{
    [Fact]
    public void BuildRunStarted_ChunksButtonsIntoActionsBlocks()
    {
        var options = Enumerable.Range(1, 30).Select(CreateOption).ToList();

        var blocks = BlockBuilder.BuildRunStarted("<@U1> started a run", options);

        Assert.IsType<SectionBlock>(blocks[0]);
        Assert.IsType<SectionBlock>(blocks[1]);

        var actions = blocks.OfType<ActionsBlock>().ToList();
        Assert.Equal(2, actions.Count);
        Assert.Equal(25, actions[0].Elements.Count);
        Assert.Equal(5, actions[1].Elements.Count);

        var last = actions[1].Elements[^1];
        Assert.Equal("30", last.Value);
        Assert.Equal(BlockBuilder.JoinOptionActionIdPrefix + "30", last.ActionId);
    }

    [Fact]
    public void BuildRunStarted_ActionIdsUniquePerBlock()
    {
        var blocks = BlockBuilder.BuildRunStarted("text", Enumerable.Range(1, 10).Select(CreateOption));

        foreach (var block in blocks.OfType<ActionsBlock>())
            Assert.Equal(block.Elements.Count, block.Elements.Select(e => e.ActionId).Distinct().Count());
    }

    [Fact]
    public void Response_SerializesBlocksAndOmitsNulls()
    {
        var response = new SlashCommandResponse
        {
            Text = "fallback",
            Type = ResponseType.Channel,
            Blocks = BlockBuilder.BuildRunStarted("hi", [CreateOption(7)])
        };

        var json = JsonNode.Parse(SlackJsonSerializer.Serialize(response))!.AsObject();

        Assert.Equal("fallback", (string?)json["text"]);
        Assert.Equal("in_channel", (string?)json["response_type"]);
        Assert.False(json.ContainsKey("thread_ts"));
        Assert.False((bool)json["replace_original"]!);
        Assert.False(json.ContainsKey("attachments"));

        var blocks = json["blocks"]!.AsArray();
        Assert.Equal("section", (string?)blocks[0]!["type"]);
        Assert.Equal("mrkdwn", (string?)blocks[0]!["text"]!["type"]);
        Assert.False(blocks[0]!["text"]!.AsObject().ContainsKey("emoji"));
        Assert.False(blocks[0]!.AsObject().ContainsKey("block_id"));

        var button = blocks[2]!["elements"]![0]!.AsObject();
        Assert.Equal("actions", (string?)blocks[2]!["type"]);
        Assert.Equal("button", (string?)button["type"]);
        Assert.Equal("join_option_7", (string?)button["action_id"]);
        Assert.Equal("7", (string?)button["value"]);
        Assert.Equal("plain_text", (string?)button["text"]!["type"]);
        Assert.False(button.ContainsKey("style"));
    }

    [Fact]
    public void BlockActionsPayload_Deserializes()
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
        Assert.Equal("message", payload.Container!.Type);
        Assert.Null(payload.Enterprise);
        Assert.Equal("https://hooks.slack.com/actions/T1/1/abc", payload.ResponseUrl);

        var action = Assert.Single(payload.Actions);
        Assert.Equal("join_option_42", action.ActionId);
        Assert.Equal("42", action.Value);
    }

    private static Option CreateOption(int id) => new()
    {
        Id = id,
        Name = "option" + id,
        CreatedBy = 1,
        CreatedDate = DateTimeOffset.UtcNow,
        GroupId = 1
    };
}
