namespace TeaTime.Common.Exceptions;

public class RunEndException(string message, RunEndException.RunEndExceptionReason reason)
    : TeaTimeException("Failed to end run. " + message)
{
    public RunEndExceptionReason Reason { get; } = reason;

    public enum RunEndExceptionReason
    {
        Unspecified = 0,

        /// <summary>
        /// No active run to end
        /// </summary>
        NoActiveRun = 1,

        /// <summary>
        /// Run has no orders
        /// </summary>
        NoOrders = 2,

        /// <summary>
        /// The current user has not joined
        /// </summary>
        NotJoined = 3
    }
}
