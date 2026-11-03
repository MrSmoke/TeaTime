namespace TeaTime.Slack;

using System.Diagnostics.CodeAnalysis;
using Common.Abstractions;
using Models;
using Models.Interactivity;
using Models.SlashCommands;

internal static class Extensions
{
    private const string CallbackDataKey = "CALLBACKDATA";

    internal static void AddCallbackState(this BaseCommand command, CallbackData callbackData)
    {
        command.State[CallbackDataKey] = callbackData;
    }

    internal static bool TryGetCallbackState(this BaseEvent command, [NotNullWhen(true)] out CallbackData? callbackData)
    {
        if (!command.State.TryGetValue(CallbackDataKey, out var value))
        {
            callbackData = null;
            return false;
        }

        if (value is CallbackData typedData)
        {
            callbackData = typedData;
            return true;
        }

        callbackData = null;
        return false;
    }

    internal static CallbackData ToCallbackData(this SlashCommand command)
    {
        return new CallbackData
        {
            UserId = command.UserId,
            ResponseUrl = command.ResponseUrl
        };
    }

    internal static CallbackData ToCallbackData(this BlockActionsPayload payload, string responseUrl)
    {
        return new CallbackData
        {
            UserId = payload.User.Id,
            ResponseUrl = responseUrl
        };
    }

    internal static string? GetUserName(this SlashCommand command)
    {
#pragma warning disable CS0618 // Read while Slack still sends it; callers fall back when missing
        return command.UserName;
#pragma warning restore CS0618
    }
}