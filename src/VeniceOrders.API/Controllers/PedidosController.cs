using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeniceOrders.Application.Commands.CreatePedido;
using VeniceOrders.Application.DTOs;
using VeniceOrders.Application.Queries.GetPedidoById;

namespace VeniceOrders.API.Controllers
{
    [ApiController]
    [Route("pedidos")]
    [Authorize]
    public sealed class PedidosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PedidosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePedidoRequestDto dto, CancellationToken ct)
        {
            var id = await _mediator.Send(new CreatePedidoCommand(dto), ct);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var pedido = await _mediator.Send(new GetPedidoByIdQuery(id), ct);
            return pedido is null ? NotFound() : Ok(pedido);
        }
    }
}


