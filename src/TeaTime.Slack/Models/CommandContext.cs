namespace TeaTime.Slack.Models;

using Common.Models.Data;
using SlashCommands;

public record CommandContext(SlashCommand Command, UserEntity User, RoomEntity Room);
