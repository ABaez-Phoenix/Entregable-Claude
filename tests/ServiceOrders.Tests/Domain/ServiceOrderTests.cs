using ServiceOrders.Domain.Entities;
using ServiceOrders.Domain.Enums;

namespace ServiceOrders.Tests.Domain;

public class ServiceOrderTests
{
    [Fact]
    public void Create_WithValidData_ReturnsOrderWithPendingStatus()
    {
        var order = ServiceOrder.Create("John Doe", "Laptop Dell XPS", "Screen flickering");

        Assert.Equal("John Doe", order.CustomerName);
        Assert.Equal("Laptop Dell XPS", order.EquipmentName);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Null(order.AssignedTechnician);
        Assert.NotEqual(Guid.Empty, order.Id);
    }

    [Theory]
    [InlineData("", "Laptop", "Problem")]
    [InlineData("John", "", "Problem")]
    [InlineData("John", "Laptop", "")]
    public void Create_WithMissingRequiredField_ThrowsArgumentException(string customer, string equipment, string problem)
    {
        Assert.Throws<ArgumentException>(() =>
            ServiceOrder.Create(customer, equipment, problem));
    }

    [Fact]
    public void AssignTechnician_WhenOrderIsPending_TransitionsToInProgress()
    {
        var order = ServiceOrder.Create("Jane Doe", "Router TP-Link", "No internet connection");

        order.AssignTechnician("Carlos Pérez");

        Assert.Equal(OrderStatus.InProgress, order.Status);
        Assert.Equal("Carlos Pérez", order.AssignedTechnician);
    }

    [Fact]
    public void AssignTechnician_WhenOrderIsAlreadyInProgress_ThrowsInvalidOperationException()
    {
        var order = ServiceOrder.Create("Jane Doe", "Router TP-Link", "No internet connection");
        order.AssignTechnician("Carlos Pérez");

        Assert.Throws<InvalidOperationException>(() =>
            order.AssignTechnician("Ana García"));
    }

    [Fact]
    public void AssignTechnician_WithEmptyName_ThrowsArgumentException()
    {
        var order = ServiceOrder.Create("Jane Doe", "Router TP-Link", "No internet connection");

        Assert.Throws<ArgumentException>(() => order.AssignTechnician(""));
    }
}
