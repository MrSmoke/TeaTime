namespace TeaTime.Common.Permissions;

public sealed class PermissionCheckResult
{
    private PermissionCheckResult(bool success, string? message)
    {
        Success = success;
        Message = message;
    }

    public bool Success { get; }
    public string? Message { get; }

    public static PermissionCheckResult Ok()
    {
        return new PermissionCheckResult(true, null);
    }

    public static PermissionCheckResult NotOk(string message)
    {
        return new PermissionCheckResult(false, message);
    }
}
