using MediatR;
using ServiceOrders.Domain.Interfaces;

namespace ServiceOrders.Application.Commands.AssignTechnician;

public class AssignTechnicianHandler : IRequestHandler<AssignTechnicianCommand>
{
    private readonly IServiceOrderRepository _repository;

    public AssignTechnicianHandler(IServiceOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(AssignTechnicianCommand request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Service order '{request.OrderId}' not found.");

        order.AssignTechnician(request.TechnicianName);

        await _repository.UpdateAsync(order, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
