namespace ServiceOrders.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of a technical service order.
/// </summary>
public enum OrderStatus
{
    /// <summary>Order has been created but no technician has been assigned yet.</summary>
    Pending = 0,

    /// <summary>A technician has been assigned and work is underway.</summary>
    InProgress = 1,

    /// <summary>The service has been completed and the order is closed.</summary>
    Closed = 2
}
