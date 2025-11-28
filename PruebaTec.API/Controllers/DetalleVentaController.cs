using Microsoft.AspNetCore.Mvc;
using PruebaTec.API.DTO;
using PruebaTec.API.DTO.Requests;
using PruebaTec.API.Services;

namespace PruebaTec.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetalleVentaController : ControllerBase
    {
        private readonly IDetalleVentaService _detalleService;

        public DetalleVentaController(IDetalleVentaService detalleService)
        {
            _detalleService = detalleService;
        }

        // POST: api/DetalleVenta
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DetalleVentaRequestDTO detalle)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var detalleCreado = await _detalleService.Save(detalle);

            if (detalleCreado != null)
                return Ok(detalleCreado); // Devuelve el detalle recién agregado
            else
                return BadRequest("No se pudo crear el detalle.");
        }


        // GET: api/DetalleVenta/venta/5
        [HttpGet("venta/{idVenta}")]
        public async Task<IActionResult> GetByVenta(int idVenta)
        {
            var detalles = await _detalleService.GetByVenta(idVenta);
            return Ok(detalles);
        }



        // DELETE: api/DetalleVenta/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _detalleService.Delete(id);
            return Ok();
        }
    }
}
