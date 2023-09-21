namespace TeaTime.Common.Models.Data
{
    public class User : BaseDataObject
    {
        public string DisplayName { get; init; } = null!;
        public string Username { get; init; } = null!;
    }
}
