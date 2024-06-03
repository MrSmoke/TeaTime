namespace TeaTime.Data.MySql.Repositories
{
    using System.Threading.Tasks;
    using Common.Abstractions.Data;
    using Common.Models;

    public class LinkRepository : BaseRepository, ILinkRepository
    {
        public LinkRepository(IMySqlConnectionFactory factory) : base(factory)
        {
        }

        public async Task<long?> GetObjectId(string link, LinkType linkType)
        {
            const string sql = "SELECT objectId FROM links WHERE linkType = @linkType AND link = @link";

            return await SingleOrDefaultAsync<long>(sql, new
            {
                linkType,
                link
            });
        }

        public Task Add(long objectId, LinkType linkType, string link)
        {
            const string sql = "INSERT INTO links (link, linkType, objectId) VALUES (@link, @linkType, @objectId)";

            return ExecuteAsync(sql, new
            {
                link,
                linkType,
                objectId
            });
        }

        public Task<string?> GetLinkAsync(long objectId, LinkType linkType)
        {
            const string sql = "SELECT link FROM links WHERE linkType = @linkType AND objectId = @objectId";

            return SingleOrDefaultAsync<string>(sql, new
            {
                linkType,
                objectId
            });
        }
    }
}
