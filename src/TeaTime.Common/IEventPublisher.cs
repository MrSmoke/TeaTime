namespace TeaTime.Common
{
    using System.Threading.Tasks;
    using Abstractions;

    public interface IEventPublisher
    {
        Task PublishAsync<T>(T eventModel) where T : IEvent;
    }
}
