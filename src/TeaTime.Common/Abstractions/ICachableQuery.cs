namespace TeaTime.Common.Abstractions
{
    using MediatR;

    /// <summary>
    /// Defines a query which can be cached
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICachableQuery<out T> : IRequest<T>
    {
    }
}