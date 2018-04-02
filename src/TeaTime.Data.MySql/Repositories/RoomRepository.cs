namespace TeaTime.Data.MySql.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Common.Abstractions.Data;
    using Common.Models.Data;

    public class RoomRepository : BaseRepository, IRoomRepository
    {
        public RoomRepository(ConnectionFactory factory) : base(factory)
        {
        }

        public Task CreateAsync(Room room)
        {
            const string sql = "INSERT INTO rooms (id, name, date_created) VALUES (@id, @name, @dateCreated)";

            return Insert(sql, room);
        }

        public Task<Room> GetAsync(long id)
        {
            const string sql = "SELECT id, name, date_created as DateCreated FROM rooms WHERE id = @id LIMIT 1";

            return SingleOrDefault<Room>(sql, new {id});
        }

        public Task CreateCurrentRunAsync(long roomId, long runId)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetCurrentRunAsync(long roomId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCurrentRunAsync(long roomId)
        {
            throw new NotImplementedException();
        }
    }
}
