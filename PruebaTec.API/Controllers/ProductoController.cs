using Microsoft.AspNetCore.Mvc;
using PruebaTec.API.Services;
using PruebaTec.API.DTO;
using PruebaTec.API.DTO.Requests;

namespace PruebaTec.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        // GET: api/Producto
        [HttpGet]
        public IActionResult Get()
        {
            var productos = _productoService.Get();
            return Ok(productos);
        }

        // GET: api/Producto/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var prod = _productoService.GetById(id);
            if (prod == null)
                return NotFound();

            return Ok(prod);
        }

        // POST: api/Producto
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] ProductoRequestDTO prod)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _productoService.Save(prod);
            return Ok();
        }


        // PUT: api/Producto/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductoRequestDTO prod)
        {
            await _productoService.Update(id, prod);
            return Ok();
        }

        // DELETE: api/Producto/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productoService.Delete(id);
            return Ok();
        }
    }
}
