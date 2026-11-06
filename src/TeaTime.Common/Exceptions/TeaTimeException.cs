namespace TeaTime.Common.Exceptions;

using System;

public class TeaTimeException(string message, Exception? innerException = null)
    : Exception(message, innerException);
