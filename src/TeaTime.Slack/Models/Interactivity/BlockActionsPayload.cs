namespace TeaTime.Slack.Models.Interactivity;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// See https://docs.slack.dev/reference/interaction-payloads/block_actions-payload
/// </summary>
public class BlockActionsPayload
{
    public const string PayloadType = "block_actions";

    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("user")]
    public required InteractionUser User { get; init; }

    /// <summary>
    /// Null for org-wide installs
    /// </summary>
    [JsonPropertyName("team")]
    public InteractionTeam? Team { get; init; }

    [JsonPropertyName("channel")]
    public InteractionChannel? Channel { get; init; }

    /// <summary>
    /// Only present when the interaction came from a message
    /// </summary>
    [JsonPropertyName("response_url")]
    public string? ResponseUrl { get; init; }

    [JsonPropertyName("actions")]
    public List<BlockAction> Actions { get; init; } = [];
}

public class InteractionUser
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("username")]
    public string? Username { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

public class InteractionTeam
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
}

public class InteractionChannel
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

public class BlockAction
{
    [JsonPropertyName("action_id")]
    public required string ActionId { get; init; }

    [JsonPropertyName("value")]
    public string? Value { get; init; }
}
