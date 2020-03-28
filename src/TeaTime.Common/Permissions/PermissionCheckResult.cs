namespace TeaTime.Common.Permissions
{
    public sealed class PermissionCheckResult
    {
        public bool Success { get; }
        public string Message { get; }

        private PermissionCheckResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static PermissionCheckResult Ok()
        {
            return new PermissionCheckResult(true, null);
        }

        public static PermissionCheckResult NotOk(string message)
        {
            return new PermissionCheckResult(false, message);
        }
    }
}