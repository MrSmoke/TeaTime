namespace TeaTime.Data.MySql.Repositories
{
    using System.Threading.Tasks;
    using Common.Abstractions.Data;
    using Common.Models;
    using Dapper;

    public class LinkRepository : BaseRepository, ILinkRepository
    {
        public LinkRepository(ConnectionFactory factory) : base(factory)
        {
        }

        public Task<T> GetObjectId<T>(string link, LinkType linkType)
        {
            const string sql = "SELECT ObjectId FROM links WHERE linkType = @linkType AND link = @link LIMIT 1";

            return GetConnection().QuerySingleAsync<T>(sql, new {linkType, link});
        }

        public async Task<bool> Add<T>(T objectId, LinkType linkType, string link)
        {
            const string sql = "INSERT INTO links (link, linkType, objectId) VALUES (@link, @linkType, @objectId)";

            var rows = await GetConnection()
                .ExecuteAsync(sql, new {link, linkType, objectId})
                .ConfigureAwait(false);

            return rows == 1;
        }
    }
}
