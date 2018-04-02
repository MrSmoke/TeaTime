namespace TeaTime.Common.Features.Runs
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Commands;
    using MediatR.Pipeline;

    public class RunPreProcessor : IRequestPreProcessor<EndRunCommand>
    {
        public Task Process(EndRunCommand request, CancellationToken cancellationToken)
        {
            if(!request.Orders.Any())
                throw new Exception("Run cannot start without any orders"); //todo: Exception

            if(!request.Orders.Any(o => o.UserId.Equals(request.UserId)))
                throw new Exception("Cannot end run without joining first"); //todo: Exception

            return Task.CompletedTask;
        }
    }
}
