using NSubstitute;
using ServiceOrders.Application.Commands.AssignTechnician;
using ServiceOrders.Domain.Entities;
using ServiceOrders.Domain.Interfaces;

namespace ServiceOrders.Tests.Commands;

public class AssignTechnicianHandlerTests
{
    private readonly IServiceOrderRepository _repository = Substitute.For<IServiceOrderRepository>();
    private readonly AssignTechnicianHandler _handler;

    public AssignTechnicianHandlerTests()
    {
        _handler = new AssignTechnicianHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenOrderExists_AssignsTechnicianSuccessfully()
    {
        var order = ServiceOrder.Create("Bob", "Printer HP", "Paper jam");
        _repository.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        _repository.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        await _handler.Handle(new AssignTechnicianCommand(order.Id, "Luis Martínez"), CancellationToken.None);

        await _repository.Received(1).UpdateAsync(order, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ThrowsKeyNotFoundException()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((ServiceOrder?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new AssignTechnicianCommand(Guid.NewGuid(), "Luis"), CancellationToken.None));
    }
}
