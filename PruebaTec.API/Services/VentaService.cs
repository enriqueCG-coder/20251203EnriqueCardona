using Microsoft.EntityFrameworkCore;
using PruebaTec.API.DTO.Responses;
using PruebaTec.API.Models;
using PruebaTec.API.Utilities;

namespace PruebaTec.API.Services
{
    public class VentaService : IVentaService
    {
        private readonly InventarioContext context;
        private readonly EventDispatcher _event;

        public VentaService(InventarioContext dbcontext, EventDispatcher eventHandler)
        {
            context = dbcontext;
            _event = eventHandler;
        }

        public IEnumerable<VentaResponseDTO> Get(int idUsuario)
        {
            try
            {
                var ventas = context.GetVentas
                    .FromSqlRaw("EXEC SP_ListarVentas @p0", idUsuario)
                    .ToList();

                return ventas.Select(v => new VentaResponseDTO
                {
                    Id = v.Id,
                    Codigo = v.Codigo,
                    Fecha = v.Fecha,
                    IdUsuario = v.IdUsuario,
                    NombreUsuario = v.NombreUsuario,
                    Total = v.Total,
                    Estado = v.Estado
                });
            }
            catch
            {
                return new List<VentaResponseDTO>();
            }
        }



        public VentaResponseDTO? GetById(int id, int idUsuario)
        {
            var venta = context.GetVentas
                .FromSqlRaw("EXEC SP_BuscarVentaPorId @p0, @p1", id, idUsuario)
                .AsEnumerable()
                .FirstOrDefault();

            if (venta == null) return null;

            return new VentaResponseDTO
            {
                Id = venta.Id,
                Codigo = venta.Codigo,
                Fecha = venta.Fecha,
                IdUsuario = venta.IdUsuario,
                NombreUsuario = venta.NombreUsuario,
                Total = venta.Total,
                Estado = venta.Estado
            };
        }



        public async Task Save(int IdUsuario)
        {
            try
            {
                var codigo = Guid.NewGuid().ToString();

                await context.Database.ExecuteSqlRawAsync(
                    "EXEC SP_InsertarVenta @p0, @p1",
                    codigo, IdUsuario);

                _event.RaiseSuccess($"Venta registrada correctamente con código {codigo}.");
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
                await context.Database.ExecuteSqlRawAsync("EXEC SP_EliminarVenta @p0", id);
                _event.RaiseSuccess($"Venta con ID {id} eliminada correctamente.");
            }
            catch (Exception ex)
            {
                _event.RaiseError(ex);
            }
        }

        public async Task<bool> Confirmar(int id)
        {
            try
            {
                await context.Database.ExecuteSqlRawAsync("EXEC SP_ConfirmarVenta @p0", id);
                _event.RaiseSuccess($"Venta {id} confirmada correctamente.");
                return true;
            }
            catch (Exception ex)
            {
                _event.RaiseError(ex);
                return false;
            }
        }
    }

    public interface IVentaService
    {
        IEnumerable<VentaResponseDTO> Get(int idUsuario);
        VentaResponseDTO? GetById(int id, int idUsuario);
        Task Save(int IdUsuario);
        Task Delete(int id);
        Task<bool> Confirmar(int id);
    }
}
