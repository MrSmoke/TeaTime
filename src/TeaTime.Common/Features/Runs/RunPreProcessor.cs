namespace TeaTime.Common.Features.Runs
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Commands;
    using Exceptions;

    public class RunPreProcessor : ICommandPreProcessor<EndRunCommand>
    {
        public Task ProcessAsync(EndRunCommand request, CancellationToken cancellationToken)
        {
            if (!request.Orders.Any())
                throw new RunEndException("Cannot end run with no orders",
                    RunEndException.RunEndExceptionReason.NoOrders);

            if (request.Orders.All(o => o.User?.Id.Equals(request.UserId) == false))
                throw new RunEndException("Cannot end run without joining first",
                    RunEndException.RunEndExceptionReason.NotJoined);

            return Task.CompletedTask;
        }
    }
}
