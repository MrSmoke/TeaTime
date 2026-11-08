namespace TeaTime.Common.Features.Runs;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using MediatR;
using Models.Data;
using Models.Domain;
using Queries;
using Rooms.Queries;

public class RunQueryHandler(
    IRunRepository runRepository,
    IOrderRepository orderRepository,
    IOptionsRepository optionsRepository,
    IUserRepository userRepository) :
    IRequestHandler<GetCurrentRunQuery, RunEntity?>,
    IRequestHandler<GetRunQuery, Run?>
{
    public Task<RunEntity?> Handle(GetCurrentRunQuery request, CancellationToken cancellationToken)
    {
        return runRepository.GetCurrentRunAsync(request.RoomId, cancellationToken);
    }

    public async Task<Run?> Handle(GetRunQuery request, CancellationToken cancellationToken)
    {
        var run = await runRepository.GetAsync(request.RunId, cancellationToken);
        if (run is null)
            return null;

        return new Run(
            Id: run.Id,
            Orders: await GetOrdersAsync(run.Id, cancellationToken),
            Ended: run.Ended,
            StartDate: run.StartTime,
            EndDate: run.EndTime
        );
    }

    private async Task<IReadOnlyList<Order>> GetOrdersAsync(long runId, CancellationToken cancellationToken)
    {
        var orders = (await orderRepository.GetOrdersAsync(runId, cancellationToken)).ToList();
        if (orders.Count == 0)
            return [];

        var users = userRepository.GetManyAsync(orders.Select(o => o.UserId), cancellationToken);
        var options = optionsRepository.GetManyAsync(orders.Select(o => o.OptionId), cancellationToken);

        var userDict = (await users).ToDictionary(k => k.Id);
        var optionsDict = (await options).ToDictionary(k => k.Id);

        return
        [
            .. orders.Select(entity => ModelMapper.Map(entity,
                    userEntity: userDict.GetValueOrDefault(entity.UserId),
                    optionEntity: optionsDict.GetValueOrDefault(entity.OptionId)
                ))
                // Order by users display name for consistent ordering
                .OrderBy(o => o.User?.DisplayName)
        ];
    }
}


public static class ModelMapper
{
    public static User Map(UserEntity entity)
    {
        return new User(Id: entity.Id, Username: entity.Username, DisplayName: entity.DisplayName);
    }

    public static Option Map(OptionEntity entity)
    {
        return new Option(Id: entity.Id, Name: entity.Name, Available: entity.Available);
    }

    public static Order Map(OrderEntity entity, OptionEntity? optionEntity, UserEntity? userEntity)
    {
        return new Order(
            Id: entity.Id,
            Option: optionEntity is null ? null : Map(optionEntity),
            User: userEntity is null ? null : Map(userEntity),
            CreatedDate: entity.CreatedDate
        );
    }
}
