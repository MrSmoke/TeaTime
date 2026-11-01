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

    [JsonPropertyName("enterprise")]
    public InteractionEnterprise? Enterprise { get; init; }

    [JsonPropertyName("api_app_id")]
    public string? ApiAppId { get; init; }

    [JsonPropertyName("trigger_id")]
    public string? TriggerId { get; init; }

    [JsonPropertyName("container")]
    public InteractionContainer? Container { get; init; }

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

    [JsonPropertyName("team_id")]
    public string? TeamId { get; init; }
}

public class InteractionTeam
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("domain")]
    public string? Domain { get; init; }
}

public class InteractionEnterprise
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

public class InteractionChannel
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

public class InteractionContainer
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("message_ts")]
    public string? MessageTs { get; init; }

    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; init; }

    [JsonPropertyName("is_ephemeral")]
    public bool? IsEphemeral { get; init; }
}

public class BlockAction
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("action_id")]
    public required string ActionId { get; init; }

    [JsonPropertyName("block_id")]
    public string? BlockId { get; init; }

    [JsonPropertyName("value")]
    public string? Value { get; init; }

    [JsonPropertyName("action_ts")]
    public string? ActionTs { get; init; }
}
