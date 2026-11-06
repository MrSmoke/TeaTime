namespace TeaTime.Common.Exceptions;

public class OptionException(string message, OptionException.OptionExceptionReason reason)
    : TeaTimeException("Failed to save option. " + message)
{
    public OptionExceptionReason Reason { get; } = reason;

    public enum OptionExceptionReason
    {
        Unspecified = 0,

        /// <summary>
        /// The group already has an option with the same name, ignoring case
        /// </summary>
        DuplicateName = 1
    }
}
