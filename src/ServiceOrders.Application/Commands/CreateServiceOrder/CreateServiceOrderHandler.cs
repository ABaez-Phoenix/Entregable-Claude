using MediatR;
using ServiceOrders.Domain.Entities;
using ServiceOrders.Domain.Interfaces;

namespace ServiceOrders.Application.Commands.CreateServiceOrder;

public class CreateServiceOrderHandler : IRequestHandler<CreateServiceOrderCommand, Guid>
{
    private readonly IServiceOrderRepository _repository;

    public CreateServiceOrderHandler(IServiceOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateServiceOrderCommand request, CancellationToken cancellationToken)
    {
        var order = ServiceOrder.Create(
            request.CustomerName,
            request.EquipmentName,
            request.ProblemDescription);

        await _repository.AddAsync(order, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
