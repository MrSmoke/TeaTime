namespace TeaTime.Slack.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandRouter;
    using CommandRouter.Exceptions;
    using Controllers;
    using FakeItEasy;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging.Abstractions;
    using Models.SlashCommands;
    using Resources;
    using Services;
    using Xunit;

    public class SlackControllerTests
    {
        [Fact]
        public async Task SlashCommandHook_BadArguments_RepliesWithUsage()
        {
            var controller = CreateController(new ParameterBindingException("Too many arguments", null, "grey"));

            var response = await RunAsync(controller, "options add tea earl grey");

            Assert.Equal(ErrorStrings.CommandBadArguments("options add tea earl grey"), response.Text);
            Assert.Equal(ResponseType.User, response.Type);
        }

        [Fact]
        public async Task SlashCommandHook_UnknownCommand_RepliesWithUnknownCommand()
        {
            var controller = CreateController(new CommandNotFoundException("nope nope"));

            var response = await RunAsync(controller, "nope nope");

            Assert.Equal(ErrorStrings.CommandUnknown("nope nope"), response.Text);
            Assert.Equal(ResponseType.User, response.Type);
        }

        private static SlackController CreateController(Exception exception)
        {
            var commandRunner = A.Fake<ICommandRunner>(o => o.Strict());
            A.CallTo(() => commandRunner.RunAsync(A<string>._, A<IReadOnlyDictionary<string, object>?>._,
                    A<CancellationToken>._))
                .ThrowsAsync(exception);

            return new SlackController(commandRunner, A.Fake<ISlackService>(o => o.Strict()),
                NullLogger<SlackController>.Instance);
        }

        private static async Task<SlashCommandResponse> RunAsync(SlackController controller, string text)
        {
            var result = await controller.SlashCommandHook(new SlashCommand
            {
                Command = "/teatime",
                Text = text,
                ResponseUrl = "https://example.com/response",
                UserId = "U1",
                ChannelId = "C1"
            });

            var ok = Assert.IsType<OkObjectResult>(result);
            return Assert.IsType<SlashCommandResponse>(ok.Value);
        }
    }
}
