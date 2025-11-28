using Microsoft.AspNetCore.Mvc;
using PruebaTec.API.Services;
using PruebaTec.API.DTO;
using PruebaTec.API.DTO.Requests;

namespace PruebaTec.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: api/Usuario
        [HttpGet]
        public IActionResult Get()
        {
            var usuarios = _usuarioService.Get();
            return Ok(usuarios);
        }

        // GET: api/Usuario/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var usuario = _usuarioService.GetById(id);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        // POST: api/Usuario
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UsuarioRequestDTO usuario)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _usuarioService.Save(usuario);
            return Ok();
        }

        // PUT: api/Usuario/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UsuarioRequestDTO usuario)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _usuarioService.Update(id, usuario);
            return Ok();
        }

        // DELETE: api/Usuario/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _usuarioService.Delete(id);
            return Ok();
        }

        // PUT: api/Usuario/toggle/{id}
        [HttpPut("toggle/{id}")]
        public async Task<IActionResult> Toggle(int id)
        {
            await _usuarioService.ToggleEstado(id);
            return Ok();
        }
    }
}
