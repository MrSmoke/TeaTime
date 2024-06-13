namespace TeaTime.Common.Abstractions
{
    using System.Threading.Tasks;
    using Permissions;

    public interface IPermissionService
    {
        Task<PermissionCheckResult> CheckAsync<T>(T request) where T : IUserCommand;
        Task CheckAsync<TQuery, TResponse>(TQuery query) where TQuery : IUserQuery<TResponse>;
    }
}
