using NSubstitute;
using ServiceOrders.Application.Queries.GetOrdersByStatus;
using ServiceOrders.Domain.Entities;
using ServiceOrders.Domain.Enums;
using ServiceOrders.Domain.Interfaces;

namespace ServiceOrders.Tests.Queries;

public class GetOrdersByStatusHandlerTests
{
    private readonly IServiceOrderRepository _repository = Substitute.For<IServiceOrderRepository>();
    private readonly GetOrdersByStatusHandler _handler;

    public GetOrdersByStatusHandlerTests()
    {
        _handler = new GetOrdersByStatusHandler(_repository);
    }

    [Fact]
    public async Task Handle_ReturnsMappedDtosForGivenStatus()
    {
        var orders = new List<ServiceOrder>
        {
            ServiceOrder.Create("Client A", "PC", "Won't boot"),
            ServiceOrder.Create("Client B", "Monitor", "No display")
        };
        _repository.GetByStatusAsync(OrderStatus.Pending, Arg.Any<CancellationToken>())
            .Returns(orders);

        var result = await _handler.Handle(new GetOrdersByStatusQuery(OrderStatus.Pending), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.All(result, dto => Assert.Equal("Pending", dto.Status));
    }

    [Fact]
    public async Task Handle_WhenNoOrdersExist_ReturnsEmptyList()
    {
        _repository.GetByStatusAsync(OrderStatus.Closed, Arg.Any<CancellationToken>())
            .Returns(new List<ServiceOrder>());

        var result = await _handler.Handle(new GetOrdersByStatusQuery(OrderStatus.Closed), CancellationToken.None);

        Assert.Empty(result);
    }
}
