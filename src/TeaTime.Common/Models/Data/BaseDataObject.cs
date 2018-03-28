namespace TeaTime.Common.Models.Data
{
    using System;

    public abstract class BaseDataObject
    {
        public long Id { get; set; }

        /// <summary>
        /// The date the object was created
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }
    }
}
