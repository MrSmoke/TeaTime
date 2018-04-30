namespace TeaTime.Common.Models.Data
{
    public class Order : BaseDataObject
    {
        public long RunId { get; set; }
        public long UserId { get; set; }
        public long OptionId { get; set; }
    }
}
