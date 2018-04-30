namespace TeaTime.Common.Models.Data
{
    public class RoomItemGroup : BaseNamedDataObject
    {
        public long RoomId { get; set; }

        /// <summary>
        /// The id of the user who created this item
        /// </summary>
        public long CreatedBy { get; set; }
    }
}
