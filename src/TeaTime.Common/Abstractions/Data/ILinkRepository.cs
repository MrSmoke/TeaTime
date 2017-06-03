namespace TeaTime.Common.Abstractions.Data
{
    using System.Threading.Tasks;
    using Models;

    public interface ILinkRepository
    {
        Task<T> GetObjectId<T>(string link, LinkType linkType);
        Task<bool> Add<T>(T objectId, LinkType linkType, string link);
    }
}
