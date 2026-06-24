using MediatR;
using ServiceOrders.Application.Common;
using ServiceOrders.Domain.Interfaces;

namespace ServiceOrders.Application.Queries.GetOrdersByStatus;

public class GetOrdersByStatusHandler : IRequestHandler<GetOrdersByStatusQuery, IReadOnlyList<ServiceOrderDto>>
{
    private readonly IServiceOrderRepository _repository;

    public GetOrdersByStatusHandler(IServiceOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ServiceOrderDto>> Handle(GetOrdersByStatusQuery request, CancellationToken cancellationToken)
    {
        var orders = await _repository.GetByStatusAsync(request.Status, cancellationToken);

        return orders.Select(o => new ServiceOrderDto(
            o.Id,
            o.CustomerName,
            o.EquipmentName,
            o.ProblemDescription,
            o.Status.ToString(),
            o.AssignedTechnician,
            o.CreatedAt,
            o.UpdatedAt
        )).ToList();
    }
}
