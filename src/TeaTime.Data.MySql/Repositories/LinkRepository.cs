namespace TeaTime.Data.MySql.Repositories
{
    using System.Threading.Tasks;
    using Common.Abstractions.Data;
    using Common.Models;

    public class LinkRepository : BaseRepository, ILinkRepository
    {
        public LinkRepository(ConnectionFactory factory) : base(factory)
        {
        }

        public Task<T> GetObjectId<T>(string link, LinkType linkType)
        {
            const string sql = "SELECT objectId FROM links WHERE linkType = @linkType AND link = @link LIMIT 1";

            return SingleOrDefaultAsync<T>(sql, new {linkType, link});
        }

        public Task Add<T>(T objectId, LinkType linkType, string link)
        {
            const string sql = "INSERT INTO links (link, linkType, objectId) VALUES (@link, @linkType, @objectId)";

            return ExecuteAsync(sql, new {link, linkType, objectId});
        }
    }
}
