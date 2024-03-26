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
                           "(id, name, roomCode, createdBy, createdDate) VALUES " +
                           "(@id, @name, @roomCode, @createdBy, @createdDate)";

        return ExecuteAsync(sql, room);
    }

    public Task UpdateAsync(Room room)
    {
        const string sql = "UPDATE rooms SET name=@name,roomCode=@roomCode WHERE id = @id";

        return ExecuteAsync(sql, room);
    }

    public Task<Room?> GetAsync(long id)
    {
        const string sql = "SELECT id, name, roomCode, createdBy, createdDate FROM rooms WHERE id = @id";

        return SingleOrDefaultAsync<Room>(sql, new { id });
    }

    public Task<Room?> GetByRoomCodeAsync(string roomCode)
    {
        const string sql = "SELECT id, name, roomCode, createdBy, createdDate FROM rooms WHERE roomCode = @roomCode";

        return SingleOrDefaultAsync<Room>(sql, new { roomCode });
    }
}
