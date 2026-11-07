namespace TeaTime.Common.Exceptions;

public class OrderException(string message, OrderException.OrderExceptionReason reason)
    : TeaTimeException("Failed to save order. " + message)
{
    public OrderExceptionReason Reason { get; } = reason;

    public enum OrderExceptionReason
    {
        Unspecified = 0,

        /// <summary>
        /// The option is unavailable or has been deleted
        /// </summary>
        OptionUnavailable = 1,

        /// <summary>
        /// The run has ended or doesn't exist, so its orders can't change
        /// </summary>
        RunEnded = 2
    }
}
