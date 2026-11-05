namespace TeaTime.Common.Features.Orders.Permissions;

using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Commands;
using Common.Permissions;

public class OrderPermissions :
    IPermissionCheck<DeleteOrderCommand>,
    IPermissionCheck<UpdateOrderOptionCommand>
{
    public Task<PermissionCheckResult> CheckAsync(DeleteOrderCommand request,
        CancellationToken cancellationToken = default)
    {
        // Only allow users to delete their own order
        if (request.Order.UserId != request.UserId)
            return Task.FromResult(PermissionCheckResult.NotOk("You cannot delete other users' orders"));

        return Task.FromResult(PermissionCheckResult.Ok());
    }

    public Task<PermissionCheckResult> CheckAsync(UpdateOrderOptionCommand request,
        CancellationToken cancellationToken = default)
    {
        // Only allow users to change their own order
        if (request.Order.UserId != request.UserId)
            return Task.FromResult(PermissionCheckResult.NotOk("You cannot change other users' orders"));

        return Task.FromResult(PermissionCheckResult.Ok());
    }
}
