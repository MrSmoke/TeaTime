namespace TeaTime.Common
{
    public interface IStartupAction
    {
        string Name { get; }
        void Execute();
    }
}
