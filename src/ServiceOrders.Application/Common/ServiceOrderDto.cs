namespace ServiceOrders.Application.Common;

public record ServiceOrderDto(
    Guid Id,
    string CustomerName,
    string EquipmentName,
    string ProblemDescription,
    string Status,
    string? AssignedTechnician,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
