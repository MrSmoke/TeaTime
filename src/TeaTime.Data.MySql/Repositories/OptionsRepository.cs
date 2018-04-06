namespace TeaTime.Data.MySql.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Abstractions.Data;
    using Common.Models.Data;

    public class OptionsRepository : BaseRepository, IOptionsRepository
    {
        private const string GroupSelectColumns = "id, name, roomId, createdBy, createdDate";
        private const string OptionSelectColumns = "id, name, groupId, createdBy, createdDate";

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

        public Task<RoomItemGroup> GetGroupAsync(long groupId)
        {
            const string sql = "SELECT " + GroupSelectColumns +
                               " FROM option_groups WHERE id = @groupId";

            return SingleOrDefaultAsync<RoomItemGroup>(sql, new {groupId});
        }

        public Task<RoomItemGroup> GetGroupByNameAsync(long roomId, string name)
        {
            const string sql = "SELECT " + GroupSelectColumns +
                               " FROM option_groups WHERE roomId = @roomId AND name = @name";

            return SingleOrDefaultAsync<RoomItemGroup>(sql, new {roomId, name});
        }

        public Task CreateAsync(Option option)
        {
            const string sql = "INSERT INTO options " +
                               "(id, name, groupId, createdBy, createdDate) VALUES " +
                               "(@id, @name, @groupId, @createdBy, @createdDate)";

            return ExecuteAsync(sql, option);
        }

        public Task<Option> GetAsync(long id)
        {
            const string sql = "SELECT " + OptionSelectColumns + " FROM options WHERE id = @id";

            return SingleOrDefaultAsync<Option>(sql, new {id});
        }

        public Task<IEnumerable<Option>> GetManyAsync(IEnumerable<long> ids)
        {
            const string sql = "SELECT " + OptionSelectColumns + " FROM options WHERE id IN @ids";

            return QueryAsync<Option>(sql, new { ids });
        }

        public Task<IEnumerable<Option>> GetOptionsByGroupIdAsync(long groupId)
        {
            const string sql = "SELECT "+ OptionSelectColumns + " FROM options WHERE groupId = @groupId";

            return QueryAsync<Option>(sql, new {groupId});
        }
    }
}
