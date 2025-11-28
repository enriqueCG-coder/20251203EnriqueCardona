using Microsoft.EntityFrameworkCore;
using PruebaTec.API.Models;
using PruebaTec.API.Utilities;
using PruebaTec.API.DTO;
using PruebaTec.API.DTO.Responses;
using PruebaTec.API.DTO.Requests;

namespace PruebaTec.API.Services
{
    public class DetalleVentaService : IDetalleVentaService
    {
        private readonly InventarioContext context;
        private readonly EventDispatcher _event;

        public DetalleVentaService(InventarioContext dbcontext, EventDispatcher eventHandler)
        {
            context = dbcontext;
            _event = eventHandler;
        }

        public async Task<DetalleVentaResponseDTO?> Save(DetalleVentaRequestDTO request)
        {
            try
            {
                decimal iva = (request.Cantidad * request.Precio) * 0.13m;
                decimal total = (request.Cantidad * request.Precio) + iva;

                var detallesList = await context.GetDetalles
    .FromSqlRaw("EXEC SP_InsertarDetalleVenta @p0, @p1, @p2, @p3, @p4, @p5",
        request.IdVenta, request.IdProducto, request.Cantidad, request.Precio, iva, total)
    .ToListAsync(); 

                var detalleInsertado = detallesList.FirstOrDefault();
                

                return new DetalleVentaResponseDTO
                {
                    Id = detalleInsertado.Id,
                    Fecha = detalleInsertado.Fecha,
                    IdVenta = detalleInsertado.IdVenta,
                    IdProducto = detalleInsertado.IdProducto,
                    NombreProducto = detalleInsertado.NombreProducto,
                    Cantidad = detalleInsertado.Cantidad,
                    Precio = detalleInsertado.Precio,
                    Iva = detalleInsertado.Iva,
                    Total = detalleInsertado.Total
                };
            }
            catch (Exception ex)
            {
                _event.RaiseError(ex);
                return null;
            }
        }




        public async Task<IEnumerable<DetalleVentaResponseDTO>> GetByVenta(int idVenta)
        {
            try
            {
                var detalles = await context.GetDetalles
                    .FromSqlRaw("EXEC SP_ListarDetallesPorVenta @p0", idVenta)
                    .ToListAsync();

                _event.RaiseSuccess("Detalles obtenidos correctamente.");

                return detalles.Select(d => new DetalleVentaResponseDTO
                {
                    Id = d.Id,
                    Fecha = d.Fecha,
                    IdVenta = d.IdVenta,
                    IdProducto = d.IdProducto,
                    NombreProducto = d.NombreProducto,
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    Iva = d.Iva,
                    Total = d.Total
                });
            }
            catch (Exception ex)
            {
                _event.RaiseError(ex);
                return new List<DetalleVentaResponseDTO>();
            }
        }


        public async Task<bool> Delete(int id)
        {
            try
            {
                await context.Database.ExecuteSqlRawAsync("EXEC SP_EliminarDetalleVenta @p0", id);
                _event.RaiseSuccess("Detalle eliminado correctamente.");
                return true;
            }
            catch (Exception ex)
            {
                _event.RaiseError(ex);
                return false;
            }
        }
    }

    public interface IDetalleVentaService
    {
        Task<DetalleVentaResponseDTO?> Save(DetalleVentaRequestDTO request);
        Task<IEnumerable<DetalleVentaResponseDTO>> GetByVenta(int idVenta);
        Task<bool> Delete(int id);
    }
}
