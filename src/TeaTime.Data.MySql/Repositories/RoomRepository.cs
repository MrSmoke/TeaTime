namespace TeaTime.Data.MySql.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Abstractions.Data;
    using Common.Models;
    using Dapper;

    public class RoomRepository : BaseRepository, IRoomRepository
    {
        public RoomRepository(ConnectionFactory factory) : base(factory)
        {
        }

        public Task<bool> Create(Room room)
        {
            const string sql = "INSERT INTO rooms (id, name, date_created) VALUES (@id, @name, @dateCreated)";

            return Insert(sql, room);
        }

        public Task<Room> Get(Guid id)
        {
            const string sql = "SELECT id, name, date_created as DateCreated FROM rooms WHERE id = @id LIMIT 1";

            return SingleOrDefault<Room>(sql, new {id});
        }

        public Task<bool> AddGroup(Group group)
        {
            const string sql = "INSERT INTO room_groups (id, name, room_id, date_created) VALUES (@id, @name, @roomId, @dateCreated)";

            return Insert(sql, group);
        }

        public Task<Group> GetGroupByName(Guid roomId, string name)
        {
            const string sql = "SELECT id, name, room_id as RoomId, date_created as DateCreated FROM room_groups WHERE room_id = @roomId AND name = @name LIMIT 1";

            return SingleOrDefault<Group>(sql, new {roomId, name});
        }

        public Task<bool> DeleteGroup(Guid groupId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddGroupOption(Option option)
        {
            const string sql = "INSERT INTO room_group_options (id, name, group_id, date_created) VALUES (@id, @name, @groupId, @dateCreated)";

            return Insert(sql, option);
        }

        public Task<IEnumerable<Option>> GetGroupOptions(Guid groupId)
        {
            const string sql = "SELECT id, text, group_id as RoomGroupId, date_created AS DateCreated FROM room_group_options WHERE group_id = @groupId";

            return GetConnection(conn => conn.QueryAsync<Option>(sql, new {groupId}));
        }

        public Task<bool> DeleteGroupOption(Guid optionId)
        {
            throw new NotImplementedException();
        }
    }
}
