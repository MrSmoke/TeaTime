namespace TeaTime.Common.Abstractions
{
    using System;
    using MediatR;

    public interface IEvent : INotification
    {
        DateTimeOffset Timestamp { get; set; }
    }
}