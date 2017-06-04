namespace TeaTime.Common.Exceptions
{
    using System;

    public class TeaTimeException : Exception
    {
        public TeaTimeException(string message ) : base(message)
        {
        }
    }
}
