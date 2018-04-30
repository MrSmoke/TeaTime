namespace TeaTime.Common
{
    using System.Threading.Tasks;
    using Abstractions;

    public interface IEventPublisher
    {
        Task Publish<T>(T @event) where T : IEvent;
    }
}