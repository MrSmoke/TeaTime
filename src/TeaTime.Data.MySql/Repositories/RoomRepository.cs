namespace TeaTime.Data.MySql.Repositories;

using System.Threading.Tasks;
using Common.Abstractions.Data;
using Common.Models.Data;
using Factories;

public class RoomRepository(IMySqlConnectionFactory factory) : BaseRepository(factory), IRoomRepository
{
    public Task CreateAsync(Room room)
    {
        const string sql = "INSERT INTO rooms " +
                           "(id, name, createdBy, createdDate) VALUES " +
                           "(@id, @name, @createdBy, @createdDate)";

        return ExecuteAsync(sql, room);
    }

    public Task<Room?> GetAsync(long id)
    {
        const string sql = "SELECT id, name, createdBy, createdDate FROM rooms WHERE id = @id";

        return SingleOrDefaultAsync<Room>(sql, new { id });
    }
}
