﻿namespace TeaTime.Common.Models
{
    using System;

    public class Room
    {
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the room
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The date the room was created
        /// </summary>
        public DateTime DateCreated { get; set; }
    }
}
