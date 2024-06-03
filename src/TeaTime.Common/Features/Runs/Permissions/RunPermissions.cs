namespace TeaTime.Common.Features.Runs.Permissions
{
    using System.Threading.Tasks;
    using Abstractions;
    using Commands;
    using Common.Permissions;

    public class RunPermissions : IPermissionCheck<EndRunCommand>
    {
        public Task<PermissionCheckResult> CheckAsync(EndRunCommand request)
        {
            //only allow person who started the run to end it
            if (request.Run.UserId != request.UserId)
                return Task.FromResult(PermissionCheckResult.NotOk("Only the person who started the round can end it"));

            return Task.FromResult(PermissionCheckResult.Ok());
        }
    }
}
