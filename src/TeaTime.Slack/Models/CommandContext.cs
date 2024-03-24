namespace TeaTime.Slack.Models;

using Common.Models.Data;
using Requests;

public record CommandContext(SlashCommand Command, User User, Room Room);
