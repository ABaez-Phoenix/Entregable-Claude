using Microsoft.EntityFrameworkCore;
using ServiceOrders.Domain.Entities;
using ServiceOrders.Domain.Enums;
using ServiceOrders.Domain.Interfaces;
using ServiceOrders.Infrastructure.Persistence;

namespace ServiceOrders.Infrastructure.Repositories;

public class ServiceOrderRepository : IServiceOrderRepository
{
    private readonly AppDbContext _context;

    public ServiceOrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.ServiceOrders.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<ServiceOrder>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
        => await _context.ServiceOrders
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(ServiceOrder order, CancellationToken cancellationToken = default)
        => await _context.ServiceOrders.AddAsync(order, cancellationToken);

    public Task UpdateAsync(ServiceOrder order, CancellationToken cancellationToken = default)
    {
        _context.ServiceOrders.Update(order);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
