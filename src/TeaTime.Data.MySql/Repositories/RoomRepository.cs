namespace TeaTime.Data.MySql.Repositories;

using System.Threading;
using System.Threading.Tasks;
using Common.Abstractions.Data;
using Common.Models.Data;
using Factories;

public class RoomRepository(IMySqlConnectionFactory factory) : BaseRepository(factory), IRoomRepository
{
    public Task CreateAsync(RoomEntity room, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO rooms " +
                           "(id, name, createdBy, createdDate) VALUES " +
                           "(@id, @name, @createdBy, @createdDate)";

        return ExecuteAsync(sql, room, cancellationToken);
    }

    public Task<RoomEntity?> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT id, name, createdBy, createdDate FROM rooms WHERE id = @id";

        return SingleOrDefaultAsync<RoomEntity>(sql, new { id }, cancellationToken);
    }
}
