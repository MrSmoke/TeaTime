namespace TeaTime.Common.Abstractions;

using System.Collections.Generic;

public abstract record BaseCommand : ICommand
{
    public Dictionary<string, object> State { get; } = new();
}
