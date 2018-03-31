namespace TeaTime.Common.Abstractions
{
    /// <summary>
    /// A query which is issued by a user
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUserQuery<out T> : IQuery<T>
    {
    }
}