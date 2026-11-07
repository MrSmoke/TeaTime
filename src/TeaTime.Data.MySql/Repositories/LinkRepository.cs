namespace TeaTime.Data.MySql.Repositories;

using System.Threading;
using System.Threading.Tasks;
using Common.Abstractions.Data;
using Common.Models;
using Factories;

public class LinkRepository(IMySqlConnectionFactory factory) : BaseRepository(factory), ILinkRepository
{
    public async Task<long?> GetObjectIdAsync(string link, LinkType linkType,
        CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT objectId FROM links WHERE linkType = @linkType AND link = @link";

        return await SingleOrDefaultAsync<long>(sql, new
        {
            linkType,
            link
        }, cancellationToken);
    }

    public Task AddAsync(long objectId, LinkType linkType, string link, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO links (link, linkType, objectId) VALUES (@link, @linkType, @objectId)";

        return ExecuteAsync(sql, new
        {
            link,
            linkType,
            objectId
        }, cancellationToken);
    }

    public Task<string?> GetLinkAsync(long objectId, LinkType linkType, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT link FROM links WHERE linkType = @linkType AND objectId = @objectId";

        return SingleOrDefaultAsync<string>(sql, new
        {
            linkType,
            objectId
        }, cancellationToken);
    }
}
