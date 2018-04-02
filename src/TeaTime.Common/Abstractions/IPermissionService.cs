namespace TeaTime.Common.Abstractions
{
    using System.Threading.Tasks;
    using Permissions;

    public interface IPermissionService
    {
        Task<PermisionCheckResult> CheckAsync<T>(T request) where T : IUserCommand;
        Task CheckAsync<TQuery, TResponse>(TQuery query) where TQuery : IUserQuery<TResponse>;
    }

    public interface IPermissionCheck<in T>
    {
        Task<PermisionCheckResult> CheckAsync(T request);
    }
}
