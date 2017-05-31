namespace TeaTime.Common.Services
{
    using System.Threading.Tasks;
    using Models;

    public interface ILinkService
    {
        Task<T> GetLinkedObjectId<T>(string link, LinkType linkType);
        Task<bool> AddLink<T>(T objectId, LinkType linkType, string link);
    }
}
