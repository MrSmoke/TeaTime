namespace TeaTime.Slack.Tests;

using System;
using Common.Features.RoomItemGroups.Models;
using Common.Models.Data;

internal static class SlackTestData
{
    public const long GroupId = 2;

    public static Option CreateOption(long id, string name, bool available = true) => new()
    {
        Id = id,
        Name = name,
        Available = available,
        GroupId = GroupId,
        CreatedBy = CommandTestHost.UserId,
        CreatedDate = DateTimeOffset.UnixEpoch
    };

    public static RoomItemGroupModel CreateGroup(string name, params Option[] options) => new()
    {
        Id = GroupId,
        Name = name,
        RoomId = CommandTestHost.RoomId,
        CreatedBy = CommandTestHost.UserId,
        CreatedDate = DateTimeOffset.UnixEpoch,
        Options = options
    };
}
