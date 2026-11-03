namespace TeaTime.Slack.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Nodes;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandRouter;
    using CommandRouter.Exceptions;
    using Controllers;
    using FakeItEasy;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging.Abstractions;
    using Models.Interactivity;
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
            Assert.Equal("ephemeral", response.ResponseType);
        }

        [Fact]
        public async Task SlashCommandHook_SslCheck_RepliesWithEmptyOk()
        {
            var controller = new SlackController(A.Fake<ICommandRunner>(o => o.Strict()),
                A.Fake<ISlackService>(o => o.Strict()), NullLogger<SlackController>.Instance);

            // Slack only sends ssl_check and token, so the usual fields bind as null
            var result = await controller.SlashCommandHook(new SlashCommand
            {
                Command = null!,
                Text = null!,
                ResponseUrl = null!,
                UserId = null!,
                ChannelId = null!,
                SslCheck = "1"
            });

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task SlashCommandHook_UnknownCommand_RepliesWithUnknownCommand()
        {
            var controller = CreateController(new CommandNotFoundException("nope nope"));

            var response = await RunAsync(controller, "nope nope");

            Assert.Equal(ErrorStrings.CommandUnknown("nope nope"), response.Text);
            Assert.Equal("ephemeral", response.ResponseType);
        }

        [Fact]
        public async Task InteractionHook_UnsupportedType_IgnoredWithEmptyOk()
        {
            var slackService = A.Fake<ISlackService>(o => o.Strict());
            var controller = new SlackController(A.Fake<ICommandRunner>(o => o.Strict()), slackService,
                NullLogger<SlackController>.Instance);

            var result = await controller.InteractionHook(new InteractionRequest
            {
                PayloadJson = """{"type":"interactive_message","user":{"id":"U1"}}"""
            });

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task InteractionHook_BlockActions_HandedToServiceAndAcknowledged()
        {
            var slackService = A.Fake<ISlackService>();
            var controller = new SlackController(A.Fake<ICommandRunner>(o => o.Strict()), slackService,
                NullLogger<SlackController>.Instance);

            var result = await controller.InteractionHook(new InteractionRequest
            {
                PayloadJson = """
                    {"type":"block_actions","user":{"id":"U1","username":"steve"},
                     "channel":{"id":"C1","name":"general"},"response_url":"https://hooks.slack.com/actions/T1/1/abc",
                     "actions":[{"type":"button","action_id":"join_option_1","value":"1"}]}
                    """
            });

            Assert.IsType<OkResult>(result);
            A.CallTo(() => slackService.HandleBlockActionsAsync(
                    A<BlockActionsPayload>.That.Matches(p => p.User.Id == "U1")))
                .MustHaveHappenedOnceExactly();
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

        private sealed record SlackReply(string? Text, string? ResponseType);

        private static async Task<SlackReply> RunAsync(SlackController controller, string text)
        {
            var result = await controller.SlashCommandHook(new SlashCommand
            {
                Command = "/teatime",
                Text = text,
                ResponseUrl = "https://example.com/response",
                UserId = "U1",
                ChannelId = "C1"
            });

            var content = Assert.IsType<ContentResult>(result);
            Assert.Equal("application/json", content.ContentType);

            var json = JsonNode.Parse(content.Content!)!;
            return new SlackReply((string?)json["text"], (string?)json["response_type"]);
        }
    }
}
