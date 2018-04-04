namespace TeaTime.Data.MySql.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Abstractions.Data;
    using Common.Models.Data;

    public class OptionsRepository : BaseRepository, IOptionsRepository
    {
        public OptionsRepository(ConnectionFactory factory) : base(factory)
        {
        }

        public Task CreateGroupAsync(RoomItemGroup group)
        {
            const string sql = "INSERT INTO option_groups " +
                               "(id, name, roomId, createdBy, createdDate) VALUES " +
                               "(@id, @name, @roomId, @createdBy, @createdDate)";

            return ExecuteAsync(sql, group);
        }

        public Task<RoomItemGroup> GetGroupByNameAsync(long roomId, string name)
        {
            const string sql =
                "SELECT id, name, roomId, createdBy, createdDate FROM option_groups WHERE roomId = @roomId AND name = @name";

            return SingleOrDefaultAsync<RoomItemGroup>(sql, new {roomId, name});
        }

        public Task CreateAsync(Option option)
        {
            const string sql = "INSERT INTO options " +
                               "(id, name, groupId, createdBy, createdDate) VALUES " +
                               "(@id, @name, @groupId, @createdBy, @createdDate)";

            return ExecuteAsync(sql, option);
        }

        public Task<IEnumerable<Option>> GetByGroupIdAsync(long groupId)
        {
            const string sql = "SELECT id, name, groupId, createdBy, createdDate FROM options WHERE groupId = @groupId";

            return QueryAsync<Option>(sql, new {groupId});
        }
    }
}
