namespace TeaTime.Common.Models.Data
{
    public class Room : BaseNamedDataObject
    {
        /// <summary>
        /// The id of the user who created this room
        /// </summary>
        public long CreatedBy { get; set; }
    }
}
