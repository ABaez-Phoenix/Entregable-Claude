using ServiceOrders.Domain.Entities;
using ServiceOrders.Domain.Enums;

namespace ServiceOrders.Domain.Interfaces;

public interface IServiceOrderRepository
{
    Task<ServiceOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ServiceOrder>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(ServiceOrder order, CancellationToken cancellationToken = default);
    Task UpdateAsync(ServiceOrder order, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
