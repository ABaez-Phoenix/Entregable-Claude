using MediatR;

namespace ServiceOrders.Application.Commands.AssignTechnician;

public record AssignTechnicianCommand(Guid OrderId, string TechnicianName) : IRequest;
