using NSubstitute;
using ServiceOrders.Application.Commands.CreateServiceOrder;
using ServiceOrders.Domain.Entities;
using ServiceOrders.Domain.Interfaces;

namespace ServiceOrders.Tests.Commands;

public class CreateServiceOrderHandlerTests
{
    private readonly IServiceOrderRepository _repository = Substitute.For<IServiceOrderRepository>();
    private readonly CreateServiceOrderHandler _handler;

    public CreateServiceOrderHandlerTests()
    {
        _handler = new CreateServiceOrderHandler(_repository);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsNewOrderId()
    {
        _repository.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var command = new CreateServiceOrderCommand("Alice Smith", "MacBook Pro", "Battery drains fast");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result);
        await _repository.Received(1).AddAsync(Arg.Any<ServiceOrder>(), Arg.Any<CancellationToken>());
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
