namespace TeaTime.Common.Abstractions
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface ICommandPreProcessor<in TCommand>
    {
        Task ProcessAsync(TCommand request, CancellationToken cancellationToken);
    }
}