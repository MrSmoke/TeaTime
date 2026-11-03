namespace TeaTime.Common.Exceptions;

public class RunStartException(string message, RunStartException.RunStartExceptionReason reason)
    : TeaTimeException("Failed to start run. " + message)
{
    public RunStartExceptionReason Reason { get; } = reason;

    public enum RunStartExceptionReason
    {
        Unspecified = 0,

        /// <summary>
        /// There is an current active run in the room
        /// </summary>
        ExistingActiveRun = 1
    }
}
