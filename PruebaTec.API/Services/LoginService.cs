using Microsoft.EntityFrameworkCore;
using PruebaTec.API.DTO.Responses;
using PruebaTec.API.Models;
using PruebaTec.API.Utilities;

namespace PruebaTec.API.Services
{
    public class LoginService : ILoginService
    {

        protected readonly InventarioContext context;
        private readonly EventDispatcher _events;

        public LoginService(InventarioContext dbcontext, EventDispatcher eventHandler)
        {
            context = dbcontext;
            _events = eventHandler;
        }


        public async Task<UsuarioResponseDTO?> Login(string nombre, string password)
        {
            try
            {
                var usuarios = await context.Usuarios
                    .FromSqlRaw("EXEC sp_Usuario_Login @p0", nombre)
                    .ToListAsync();

                if (usuarios.Count == 0)
                {
                    _events.RaiseFailed("Usuario no encontrado.");
                    return null;
                }

                var usuario = usuarios.First();

                if (!BCrypt.Net.BCrypt.Verify(password, usuario.Pwd))
                {
                    _events.RaiseFailed("Usuario o contraseña inválidos.");
                    return null;
                }

                _events.RaiseSuccess("Login exitoso.");

                return new UsuarioResponseDTO
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Rol = usuario.Rol
                };
            }
            catch (Exception ex)
            {
                _events.RaiseError(ex);
                return null;
            }
        }
    }


    public interface ILoginService
    {
        Task<UsuarioResponseDTO?> Login(string nombre, string password);
    }
}
