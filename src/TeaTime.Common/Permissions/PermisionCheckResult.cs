namespace TeaTime.Common.Permissions
{
    public class PermisionCheckResult
    {
        public bool Success { get; }
        public string Message { get; }

        private PermisionCheckResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static PermisionCheckResult Ok()
        {
            return new PermisionCheckResult(true, null);
        }

        public static PermisionCheckResult NotOk(string message)
        {
            return new PermisionCheckResult(false, message);
        }
    }
}