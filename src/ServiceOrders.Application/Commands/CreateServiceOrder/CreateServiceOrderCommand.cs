using MediatR;

namespace ServiceOrders.Application.Commands.CreateServiceOrder;

public record CreateServiceOrderCommand(
    string CustomerName,
    string EquipmentName,
    string ProblemDescription
) : IRequest<Guid>;
