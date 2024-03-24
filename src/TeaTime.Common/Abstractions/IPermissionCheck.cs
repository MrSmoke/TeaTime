namespace TeaTime.Common.Abstractions
{
    using System.Threading.Tasks;
    using Permissions;

    public interface IPermissionCheck<in T>
    {
        Task<PermissionCheckResult> CheckAsync(T request);
    }
}
