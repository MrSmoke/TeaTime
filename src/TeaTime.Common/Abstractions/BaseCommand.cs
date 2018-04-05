namespace TeaTime.Common.Abstractions
{
    using System.Collections.Generic;

    public abstract class BaseCommand : ICommand
    {
        public Dictionary<string, string> State { get; set; } = new Dictionary<string, string>();
    }
}
