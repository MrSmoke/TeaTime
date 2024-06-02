namespace TeaTime.Common.Abstractions;

using MediatR;

public interface IQuery<out T> : IRequest<T>;
