namespace TeaTime.Slack
{
    using System.Diagnostics.CodeAnalysis;
    using Common.Abstractions;
    using Models;
    using Models.Requests;
    using Models.Requests.InteractiveMessages;

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

        internal static CallbackData ToCallbackData(this MessageRequestPayload payload)
        {
            return new CallbackData
            {
                UserId = payload.User.Id,
                ResponseUrl = payload.ResponseUrl
            };
        }
    }
}
