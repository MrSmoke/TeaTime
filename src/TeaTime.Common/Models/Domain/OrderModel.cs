namespace TeaTime.Common.Models.Domain
{
    using Data;

    public class OrderModel : BaseDataObject
    {
        public Run? Run { get; set; }
        public User? User { get; set; }
        public Option? Option { get; set; }
    }
}
