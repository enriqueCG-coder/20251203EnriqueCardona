using Microsoft.EntityFrameworkCore;
using PruebaTec.API.Models;
using PruebaTec.API.Utilities;
using PruebaTec.API.DTO.Responses;
using PruebaTec.API.DTO.Requests;

namespace PruebaTec.API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly InventarioContext context;
        private readonly EventDispatcher _event;

        public UsuarioService(InventarioContext dbcontext, EventDispatcher eventHandler)
        {
            context = dbcontext;
            _event = eventHandler;
        }

        public IEnumerable<UsuarioResponseDTO> Get()
        {
            try
            {
                var usuarios = context.Usuarios
                    .FromSqlRaw("EXEC sp_Usuarios")
                    .ToList();

                _event.RaiseSuccess("Usuarios obtenidos correctamente.");

                return usuarios.Select(u => new UsuarioResponseDTO
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Rol = u.Rol,
                    IsActive = u.IsActive
                });
            }
            catch (Exception ex)
            {
                _event.RaiseError(ex);
                return new List<UsuarioResponseDTO>();
            }
        }


        public UsuarioResponseDTO? GetById(int id)
        {
            try
            {
                var usuario = context.Usuarios
                    .FromSqlRaw("EXEC sp_Usuario_ID @p0", id)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (usuario == null)
                {
                    _event.RaiseFailed($"Usuario con ID {id} no encontrado.");
                    return null;
                }

                _event.RaiseSuccess($"Usuario {usuario.Nombre} cargado correctamente.");

                return new UsuarioResponseDTO
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Rol = usuario.Rol,
                    IsActive = usuario.IsActive
                };
            }
            catch (Exception ex)
            {
                _event.RaiseError(ex);
                return null;
            }
        }


        public async Task Save(UsuarioRequestDTO user)
        {
            try
            {
                var hashedPwd = BCrypt.Net.BCrypt.HashPassword(user.Pwd);

                await context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Usuario_Insertar @p0, @p1, @p2",
                    user.Nombre, hashedPwd, (int)user.Rol);

                _event.RaiseSuccess($"Usuario {user.Nombre} guardado correctamente.");
            }
            catch (Exception ex)
            {
                _event.RaiseError(ex);
            }
        }


        public async Task Update(int id, UsuarioRequestDTO user)
        {
            try
            {
                var hashedPwd = string.IsNullOrWhiteSpace(user.Pwd)
                    ? (await context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id)).Pwd
                    : BCrypt.Net.BCrypt.HashPassword(user.Pwd);

                await context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Usuario_Modificar @p0, @p1, @p2, @p3",
                    id, user.Nombre, hashedPwd, (int)user.Rol);

                _event.RaiseSuccess($"Usuario {user.Nombre} actualizado correctamente.");
            }
            catch (Exception ex)
            {
                _event.RaiseError(ex);
            }
        }


        public async Task Delete(int id)
        {
            try
            {
                await context.Database.ExecuteSqlRawAsync("EXEC sp_Usuario_Eliminar @p0", id);
                _event.RaiseSuccess($"Usuario con ID {id} eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _event.RaiseError(ex);
            }
        }

        public async Task ToggleEstado(int id)
        {
            try
            {
                var usuario = await context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    _event.RaiseFailed($"Usuario con ID {id} no encontrado.");
                    return;
                }

                var proc = usuario.IsActive ? "sp_Usuario_Inactivar" : "sp_Usuario_Activar";
                await context.Database.ExecuteSqlRawAsync($"EXEC {proc} @p0", id);

                _event.RaiseSuccess($"Usuario {usuario.Nombre} {(usuario.IsActive ? "desactivado" : "activado")} correctamente.");
            }
            catch (Exception ex)
            {
                _event.RaiseError(ex);
            }
        }
    }

    public interface IUsuarioService
    {
        IEnumerable<UsuarioResponseDTO> Get();
        UsuarioResponseDTO? GetById(int id);
        Task Save(UsuarioRequestDTO user);
        Task Update(int id, UsuarioRequestDTO user);
        Task Delete(int id);
        Task ToggleEstado(int id);
    }
}
