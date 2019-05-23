namespace TeaTime.Common.Options
{
    using System;

    public class InvalidOptionException : Exception
    {
        public string OptionName { get; }

        public InvalidOptionException(string optionName, string message) : base($"Invalid option {optionName}. {message}")
        {
            OptionName = optionName;
        }
    }
}
