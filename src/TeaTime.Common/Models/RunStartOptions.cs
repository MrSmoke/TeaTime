namespace TeaTime.Common.Models
{
    using System;

    public class RunStartOptions
    {
        /// <summary>
        /// The duration of the run until it is automatically ended. If null the run will need to be manually ended
        /// </summary>
        public TimeSpan? Duration { get; set; }
    }
}
