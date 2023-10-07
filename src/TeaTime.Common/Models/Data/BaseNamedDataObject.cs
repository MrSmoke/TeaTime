namespace TeaTime.Common.Models.Data
{
    public abstract class BaseNamedDataObject : BaseDataObject
    {
        /// <summary>
        /// The name of the object
        /// </summary>
        public string Name { get; set; } = null!;
    }
}
