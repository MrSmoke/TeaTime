namespace TeaTime.Data.MySql.Repositories
{
    using Common.Abstractions.Data;

    public class RoomRepository : BaseRepository, IRoomRepository
    {
        public RoomRepository(ConnectionFactory factory) : base(factory)
        {
        }
    }
}
