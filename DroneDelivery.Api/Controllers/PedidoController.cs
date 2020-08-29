using DroneDelivery.Application.Commands.Pedidos;
using DroneDelivery.Application.Interfaces;
using DroneDelivery.Application.Models;
using DroneDelivery.Application.Queries.Pedidos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DroneDelivery.Api.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PedidoController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PedidoModel>>> ObterTodos()
        {
            var response = await _mediator.Send(new ListarPedidosQuery());
            if (response.HasFails)
                return BadRequest(response.Fails);

            return Ok(response.Data);
        }


        /// <summary>
        /// Criar um pedido
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/pedido
        ///     {
        ///        "peso": 10,
        ///        "latitude": -23.5753639,
        ///        "longitude": -46.692417
        ///     }
        ///
        /// </remarks>        
        /// <param name="command"></param>  
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Adicionar(CriarPedidoCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.HasFails)
                return BadRequest(response.Fails);

            return Ok();
        }


    }
}
