namespace TeaTime.Common.Permissions;

public sealed class PermissionCheckResult
{
    private static readonly PermissionCheckResult OkResult = new(true, null);

    private PermissionCheckResult(bool success, string? message)
    {
        Success = success;
        Message = message;
    }

    public bool Success { get; }
    public string? Message { get; }

    public static PermissionCheckResult Ok()
    {
        return OkResult;
    }

    public static PermissionCheckResult NotOk(string message)
    {
        return new PermissionCheckResult(false, message);
    }
}
