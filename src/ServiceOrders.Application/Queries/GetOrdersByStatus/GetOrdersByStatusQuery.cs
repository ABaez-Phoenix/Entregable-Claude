using MediatR;
using ServiceOrders.Application.Common;
using ServiceOrders.Domain.Enums;

namespace ServiceOrders.Application.Queries.GetOrdersByStatus;

public record GetOrdersByStatusQuery(OrderStatus Status) : IRequest<IReadOnlyList<ServiceOrderDto>>;
