using ServiceOrders.Domain.Enums;

namespace ServiceOrders.Domain.Entities;

/// <summary>
/// Aggregate root representing a technical service order.
/// Encapsulates all business rules related to order lifecycle management.
/// </summary>
public class ServiceOrder
{
    /// <summary>Unique identifier for the service order.</summary>
    public Guid Id { get; private set; }

    /// <summary>Full name of the customer who requested the service.</summary>
    public string CustomerName { get; private set; } = string.Empty;

    /// <summary>Name or model of the equipment to be serviced.</summary>
    public string EquipmentName { get; private set; } = string.Empty;

    /// <summary>Description of the problem reported by the customer.</summary>
    public string ProblemDescription { get; private set; } = string.Empty;

    /// <summary>Current status of the order in its lifecycle.</summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Name of the technician assigned to this order.
    /// Null when the order is still in <see cref="OrderStatus.Pending"/> state.
    /// </summary>
    public string? AssignedTechnician { get; private set; }

    /// <summary>UTC timestamp when the order was created.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>UTC timestamp of the last update to this order.</summary>
    public DateTime UpdatedAt { get; private set; }

    private ServiceOrder() { }

    /// <summary>
    /// Creates a new service order in <see cref="OrderStatus.Pending"/> state.
    /// </summary>
    /// <param name="customerName">Full name of the customer.</param>
    /// <param name="equipmentName">Equipment model or name.</param>
    /// <param name="problemDescription">Description of the reported problem.</param>
    /// <exception cref="ArgumentException">Thrown when any required field is null or empty.</exception>
    public static ServiceOrder Create(string customerName, string equipmentName, string problemDescription)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name is required.", nameof(customerName));
        if (string.IsNullOrWhiteSpace(equipmentName))
            throw new ArgumentException("Equipment name is required.", nameof(equipmentName));
        if (string.IsNullOrWhiteSpace(problemDescription))
            throw new ArgumentException("Problem description is required.", nameof(problemDescription));

        return new ServiceOrder
        {
            Id = Guid.NewGuid(),
            CustomerName = customerName.Trim(),
            EquipmentName = equipmentName.Trim(),
            ProblemDescription = problemDescription.Trim(),
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Assigns a technician to this order and transitions it to <see cref="OrderStatus.InProgress"/>.
    /// </summary>
    /// <param name="technicianName">Name of the technician being assigned.</param>
    /// <exception cref="ArgumentException">Thrown when technician name is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the order is not in Pending state.</exception>
    public void AssignTechnician(string technicianName)
    {
        if (string.IsNullOrWhiteSpace(technicianName))
            throw new ArgumentException("Technician name is required.", nameof(technicianName));
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot assign technician to an order with status '{Status}'.");

        AssignedTechnician = technicianName.Trim();
        Status = OrderStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }
}
