namespace TeaTime.Common.Exceptions;

public class RunEndException : TeaTimeException
{
    public RunEndException(string message, RunEndExceptionReason reason) : base("Failed to end run. " + message)
    {
        Reason = reason;
    }

    public RunEndExceptionReason Reason { get; }

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
