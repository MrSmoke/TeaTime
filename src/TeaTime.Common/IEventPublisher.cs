namespace TeaTime.Common;

using System.Threading;
using System.Threading.Tasks;
using Abstractions;

public interface IEventPublisher
{
    Task PublishAsync<T>(T eventModel, CancellationToken cancellationToken = default) where T : IEvent;
}
