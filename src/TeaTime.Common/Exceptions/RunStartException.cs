namespace TeaTime.Common.Exceptions;

public class RunStartException : TeaTimeException
{
    public RunStartException(string message, RunStartExceptionReason reason) : base("Failed to start run. " + message)
    {
        Reason = reason;
    }

    public RunStartExceptionReason Reason { get; }

    public enum RunStartExceptionReason
    {
        Unspecified = 0,

        /// <summary>
        /// There is an current active run in the room
        /// </summary>
        ExistingActiveRun = 1
    }
}
