using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceOrders.Application.Commands.AssignTechnician;
using ServiceOrders.Application.Commands.CreateServiceOrder;
using ServiceOrders.Application.Queries.GetOrdersByStatus;
using ServiceOrders.Domain.Enums;

namespace ServiceOrders.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ServiceOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServiceOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Creates a new technical service order.</summary>
    /// <param name="request">Order creation data.</param>
    /// <returns>The ID of the newly created order.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateServiceOrderRequest request)
    {
        var command = new CreateServiceOrderCommand(
            request.CustomerName,
            request.EquipmentName,
            request.ProblemDescription);

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetByStatus), new { status = "Pending" }, new { id });
    }

    /// <summary>Assigns a technician to a service order.</summary>
    /// <param name="id">The order identifier.</param>
    /// <param name="request">Technician assignment data.</param>
    [HttpPatch("{id:guid}/assign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignTechnicianRequest request)
    {
        try
        {
            await _mediator.Send(new AssignTechnicianCommand(id, request.TechnicianName));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Returns all service orders filtered by status.</summary>
    /// <param name="status">Order status: Pending, InProgress, or Closed.</param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByStatus([FromQuery] string status = "Pending")
    {
        if (!Enum.TryParse<OrderStatus>(status, ignoreCase: true, out var parsedStatus))
            return BadRequest(new { error = $"Invalid status '{status}'. Valid values: Pending, InProgress, Closed." });

        var orders = await _mediator.Send(new GetOrdersByStatusQuery(parsedStatus));
        return Ok(orders);
    }
}

public record CreateServiceOrderRequest(string CustomerName, string EquipmentName, string ProblemDescription);
public record AssignTechnicianRequest(string TechnicianName);
