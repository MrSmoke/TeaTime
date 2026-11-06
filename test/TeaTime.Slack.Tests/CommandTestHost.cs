namespace TeaTime.Slack.Tests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommandRouter;
using CommandRouter.Extensions;
using Common.Abstractions;
using Common.Models.Data;
using FakeItEasy;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Models.SlashCommands;
using Services;

internal static class CommandTestHost
{
    public const long UserId = 5;
    public const long RoomId = 3;

    // Unset dependencies are strict fakes, so a test fails if a command touches them
    public static ServiceProvider CreateProvider(ISlackService? slackService = null, IMediator? mediator = null)
    {
        var services = new ServiceCollection();
        services.AddCommandRouter(typeof(ServiceCollectionExtension).Assembly);
        services.AddLogging();
        services.AddSingleton(slackService ?? A.Fake<ISlackService>(o => o.Strict()));
        services.AddSingleton(mediator ?? A.Fake<IMediator>(o => o.Strict()));
        services.AddSingleton(A.Fake<IIdGenerator<long>>(o => o.Strict()));
        services.AddSingleton(TimeProvider.System);

        return services.BuildServiceProvider();
    }

    // Every command resolves to the same user and room
    public static ISlackService CreateSlackService()
    {
        var now = DateTimeOffset.UnixEpoch;

        var user = new User { Id = UserId, Username = "slack_U1", DisplayName = "steve", CreatedDate = now };
        var room = new Room { Id = RoomId, Name = "general", CreatedBy = UserId, CreatedDate = now };

        var slackService = A.Fake<ISlackService>(o => o.Strict());

        A.CallTo(() => slackService.GetOrCreateUserAsync(A<string>._, A<string?>._, A<CancellationToken>._))
            .Returns(user);

        A.CallTo(() => slackService.GetOrCreateRoomAsync(A<string>._, A<string?>._, A<long>._, A<CancellationToken>._))
            .Returns(room);

        return slackService;
    }

    public static async Task<string> RunAsync(ServiceProvider provider, string command)
    {
        var slashCommand = new SlashCommand
        {
            Command = "/teatime",
            Text = command,
            ResponseUrl = "https://hooks.slack.com/commands/T1/1/abc",
            UserId = "U1",
            UserName = "steve",
            ChannelId = "C1",
            ChannelName = "general"
        };

        var items = new Dictionary<string, object>
        {
            { Constants.CommandContextKeys.SlashCommand, slashCommand }
        };

        var result = await provider.GetRequiredService<ICommandRunner>().RunAsync(command, items);

        using var stream = new MemoryStream();
        await result.ExecuteAsync(stream);

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    // Parses the JSON Slack receives, since Type isn't serialised on the response model
    public static async Task<JsonDocument> RunJsonAsync(ServiceProvider provider, string command)
    {
        return JsonDocument.Parse(await RunAsync(provider, command));
    }
}
