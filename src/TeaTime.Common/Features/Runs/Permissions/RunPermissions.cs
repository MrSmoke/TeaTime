namespace TeaTime.Common.Features.Runs.Permissions
{
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;
    using Commands;
    using Common.Permissions;

    public class RunPermissions : IPermissionCheck<EndRunCommand>
    {
        private readonly IRunRepository _runRepository;

        public RunPermissions(IRunRepository runRepository)
        {
            _runRepository = runRepository;
        }

        public async Task<PermissionCheckResult> CheckAsync(EndRunCommand request)
        {
            var run = await _runRepository.GetAsync(request.RunId);
            if (run == null)
                return PermissionCheckResult.Ok();

            //only allow person who started the run to end it
            if (run.UserId != request.UserId)
                return PermissionCheckResult.NotOk("Only the person who started the round can end it");

            return PermissionCheckResult.Ok();
        }
    }
}
