using Microsoft.AspNetCore.Mvc;
using PruebaTec.API.Services;

namespace PruebaTec.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentaController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        // GET: api/Venta/usuario/{idUsuario}
        [HttpGet("usuario/{idUsuario}")]
        public IActionResult GetByUsuario(int idUsuario)
        {
            var ventas = _ventaService.Get(idUsuario);
            return Ok(ventas);
        }

        // GET: api/Venta/{id}/usuario/{idUsuario}
        [HttpGet("{id}/usuario/{idUsuario}")]
        public IActionResult GetById(int id, int idUsuario)
        {
            var venta = _ventaService.GetById(id, idUsuario);
            if (venta == null)
                return NotFound();

            return Ok(venta);
        }


        // POST: api/Venta
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(int id)
        {
            await _ventaService.Save(id);
            return Ok();
        }

        // DELETE: api/Venta/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _ventaService.Delete(id);
            return Ok();
        }

        [HttpPost("confirmar/{id}")]
        public async Task<IActionResult> ConfirmarVenta(int id)
        {
            var resultado = await _ventaService.Confirmar(id);

            if (!resultado)
                return BadRequest("No se pudo confirmar la venta.");

            return Ok("Venta confirmada correctamente.");
        }
    }
}
