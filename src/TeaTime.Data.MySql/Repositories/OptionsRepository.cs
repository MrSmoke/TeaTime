namespace TeaTime.Data.MySql.Repositories;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Abstractions.Data;
using Common.Models.Data;
using Factories;

public class OptionsRepository(IMySqlConnectionFactory factory, TimeProvider systemClock)
    : BaseRepository(factory), IOptionsRepository
{
    private const string GroupSelectColumns = "id, name, roomId, createdBy, createdDate";
    private const string OptionSelectColumns = "id, name, available, groupId, createdBy, createdDate";

    public Task CreateGroupAsync(RoomItemGroupEntity group, CancellationToken cancellationToken = default)
    {
        const string sql = """
                           INSERT INTO option_groups (id, name, roomId, createdBy, createdDate)
                           VALUES (@id, @name, @roomId, @createdBy, @createdDate)
                           """;

        return ExecuteAsync(sql, group, cancellationToken);
    }

    public Task UpdateGroupAsync(RoomItemGroupEntity group, CancellationToken cancellationToken = default)
    {
        const string sql = """
                           UPDATE option_groups
                           SET name = @name
                           WHERE id = @id AND deleted = 0
                           """;

        return ExecuteAsync(sql, group, cancellationToken);
    }

    public Task DeleteGroupAsync(long groupId, CancellationToken cancellationToken = default)
    {
        const string sql = """
                           UPDATE option_groups
                           SET deleted = 1, deletedDate = @now
                           WHERE id = @groupId AND deleted = 0
                           """;

        return ExecuteAsync(sql, new
        {
            groupId,
            now = systemClock.GetUtcNow()
        }, cancellationToken);
    }

    public Task<RoomItemGroupEntity?> GetGroupAsync(long groupId, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
                           SELECT {GroupSelectColumns} FROM option_groups
                           WHERE id = @groupId AND deleted = 0
                           """;

        return SingleOrDefaultAsync<RoomItemGroupEntity>(sql, new { groupId }, cancellationToken);
    }

    public Task<RoomItemGroupEntity?> GetGroupByNameAsync(long roomId, string name,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"""
                           SELECT {GroupSelectColumns} FROM option_groups
                           WHERE roomId = @roomId AND name = @name AND deleted = 0
                           """;

        return SingleOrDefaultAsync<RoomItemGroupEntity>(sql, new { roomId, name }, cancellationToken);
    }

    public Task CreateAsync(OptionEntity option, CancellationToken cancellationToken = default)
    {
        const string sql = """
                           INSERT INTO options (id, name, available, groupId, createdBy, createdDate)
                           VALUES (@id, @name, @available, @groupId, @createdBy, @createdDate)
                           """;

        return ExecuteAsync(sql, option, cancellationToken);
    }

    public Task<OptionEntity?> GetAsync(long optionId, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
                           SELECT {OptionSelectColumns} FROM options
                           WHERE id = @optionId AND deleted = 0
                           """;

        return SingleOrDefaultAsync<OptionEntity>(sql, new { optionId }, cancellationToken);
    }

    public Task UpdateAsync(OptionEntity option, CancellationToken cancellationToken = default)
    {
        const string sql = """
                           UPDATE options
                           SET name = @name
                           WHERE id = @id AND deleted = 0
                           """;

        return ExecuteAsync(sql, option, cancellationToken);
    }

    public Task SetAvailabilityAsync(long optionId, bool available, CancellationToken cancellationToken = default)
    {
        const string sql = """
                           UPDATE options
                           SET available = @available
                           WHERE id = @optionId AND deleted = 0
                           """;

        return ExecuteAsync(sql, new { optionId, available }, cancellationToken);
    }

    public Task DeleteAsync(long optionId, CancellationToken cancellationToken = default)
    {
        const string sql = """
                           UPDATE options
                           SET deleted = 1, deletedDate = @now
                           WHERE id = @optionId AND deleted = 0
                           """;

        return ExecuteAsync(sql, new
        {
            optionId,
            now = systemClock.GetUtcNow()
        }, cancellationToken);
    }

    public Task<IEnumerable<OptionEntity>> GetManyAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
                           SELECT {OptionSelectColumns} FROM options
                           WHERE id IN @ids AND deleted = 0
                           """;

        return QueryAsync<OptionEntity>(sql, new { ids }, cancellationToken);
    }

    public Task<IEnumerable<OptionEntity>> GetOptionsByGroupIdAsync(long groupId,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"""
                           SELECT {OptionSelectColumns} FROM options
                           WHERE groupId = @groupId AND deleted = 0
                           """;

        return QueryAsync<OptionEntity>(sql, new { groupId }, cancellationToken);
    }
}
