using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using PruebaTec.API.DTO;
using PruebaTec.API.DTO.Requests;
using PruebaTec.API.Services;

namespace PruebaTec.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly JwtService _jwtService;

        public LoginController(ILoginService loginService, JwtService jwtService)
        {
            _loginService = loginService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO m)
        {
            var usuario = await _loginService.Login(m.Nombre, m.Password);

            if (usuario == null)
                return Unauthorized("Usuario o contraseña incorrectos");

            var token = _jwtService.GenerateToken(usuario.Id, usuario.Nombre, usuario.Rol.ToString());

            return Ok(new
            {
                token,
                usuario = new { usuario.Id, usuario.Nombre, usuario.Rol }
            });
        }
    }
}
