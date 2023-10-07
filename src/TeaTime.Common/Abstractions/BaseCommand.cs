namespace TeaTime.Common.Abstractions
{
    using System.Collections.Generic;

    public abstract class BaseCommand : ICommand
    {
        public Dictionary<string, object> State { get; set; } = new();
    }
}
