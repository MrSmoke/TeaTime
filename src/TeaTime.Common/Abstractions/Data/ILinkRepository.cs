namespace TeaTime.Common.Abstractions.Data
{
    using System.Threading.Tasks;
    using Models;

    public interface ILinkRepository
    {
        Task<long> GetObjectId(string link, LinkType linkType);
        Task Add(long objectId, LinkType linkType, string link);
    }
}
