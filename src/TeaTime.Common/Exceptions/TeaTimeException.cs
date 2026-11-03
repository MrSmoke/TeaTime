namespace TeaTime.Common.Exceptions;

using System;

public class TeaTimeException(string message) : Exception(message);
