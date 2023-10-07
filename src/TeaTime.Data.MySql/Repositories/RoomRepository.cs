namespace TeaTime.Data.MySql.Repositories
{
    using System.Threading.Tasks;
    using Common.Abstractions.Data;
    using Common.Models.Data;

    public class RoomRepository : BaseRepository, IRoomRepository
    {
        public RoomRepository(IMySqlConnectionFactory factory) : base(factory)
        {
        }

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

            return SingleOrDefaultAsync<Room>(sql, new {id});
        }
    }
}
