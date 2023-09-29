namespace TeaTime.Data.MySql.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Abstractions;
    using Common.Abstractions.Data;
    using Common.Models.Data;

    public class OptionsRepository : BaseRepository, IOptionsRepository
    {
        private readonly ISystemClock _systemClock;
        private const string GroupSelectColumns = "id, name, roomId, createdBy, createdDate";
        private const string OptionSelectColumns = "id, name, groupId, createdBy, createdDate";

        public OptionsRepository(IMySqlConnectionFactory factory, ISystemClock systemClock) : base(factory)
        {
            _systemClock = systemClock;
        }

        public Task CreateGroupAsync(RoomItemGroup group)
        {
            const string sql = "INSERT INTO option_groups " +
                               "(id, name, roomId, createdBy, createdDate) VALUES " +
                               "(@id, @name, @roomId, @createdBy, @createdDate)";

            return ExecuteAsync(sql, group);
        }

        public Task UpdateGroupAsync(RoomItemGroup group)
        {
            const string sql = "UPDATE option_groups SET name = @name WHERE id = @id AND deleted = 0";

            return ExecuteAsync(sql, group);
        }

        public Task DeleteGroupAsync(long groupId)
        {
            const string sql = "UPDATE option_groups" +
                               " SET deleted = 1" +
                               ",deletedDate = @now" +
                               " WHERE id = @groupId AND deleted = 0";

            return ExecuteAsync(sql, new
            {
                groupId,
                now = _systemClock.UtcNow().UtcDateTime
            });
        }

        public Task<RoomItemGroup?> GetGroupAsync(long groupId)
        {
            const string sql = "SELECT " + GroupSelectColumns +
                               " FROM option_groups " +
                               " WHERE id = @groupId AND deleted = 0";

            return SingleOrDefaultAsync<RoomItemGroup>(sql, new {groupId});
        }

        public Task<RoomItemGroup?> GetGroupByNameAsync(long roomId, string name)
        {
            const string sql = "SELECT " + GroupSelectColumns +
                               " FROM option_groups" +
                               " WHERE roomId = @roomId AND name = @name AND deleted = 0";

            return SingleOrDefaultAsync<RoomItemGroup>(sql, new {roomId, name});
        }

        public Task CreateAsync(Option option)
        {
            const string sql = "INSERT INTO options " +
                               "(id, name, groupId, createdBy, createdDate) VALUES " +
                               "(@id, @name, @groupId, @createdBy, @createdDate)";

            return ExecuteAsync(sql, option);
        }

        public Task<Option?> GetAsync(long optionId)
        {
            const string sql = "SELECT " + OptionSelectColumns +
                               " FROM options" +
                               " WHERE id = @optionId AND deleted = 0";

            return SingleOrDefaultAsync<Option>(sql, new { optionId });
        }

        public Task UpdateAsync(Option option)
        {
            const string sql = "UPDATE options" +
                               " SET name = @name" +
                               " WHERE id = @id AND deleted = 0";

            return ExecuteAsync(sql, option);
        }

        public Task DeleteAsync(long optionId)
        {
            const string sql = "UPDATE options" +
                               " SET deleted=1,deletedDate=@now" +
                               " WHERE id = @optionId AND deleted = 0";

            return ExecuteAsync(sql, new
            {
                optionId,
                now = _systemClock.UtcNow().UtcDateTime
            });
        }

        public Task<IEnumerable<Option>> GetManyAsync(IEnumerable<long> ids)
        {
            const string sql = "SELECT " + OptionSelectColumns +
                               " FROM options" +
                               " WHERE id IN @ids AND deleted = 0";

            return QueryAsync<Option>(sql, new { ids });
        }

        public Task<IEnumerable<Option>> GetOptionsByGroupIdAsync(long groupId)
        {
            const string sql = "SELECT " + OptionSelectColumns +
                               " FROM options" +
                               " WHERE groupId = @groupId AND deleted = 0";

            return QueryAsync<Option>(sql, new {groupId});
        }
    }
}
