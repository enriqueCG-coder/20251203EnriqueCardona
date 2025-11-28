using Microsoft.EntityFrameworkCore;
using PruebaTec.API.DTO;
using PruebaTec.API.DTO.Requests;
using PruebaTec.API.DTO.Responses;
using PruebaTec.API.Models;
using PruebaTec.API.Utilities;

namespace PruebaTec.API.Services
{
    public class ProductoService : IProductoService
    {
        private readonly InventarioContext context;
        private readonly EventDispatcher _event;

        public ProductoService(InventarioContext dbcontext, EventDispatcher eventHandler)
        {
            context = dbcontext;
            _event = eventHandler;
        }

        public IEnumerable<ProductoResponseDTO> Get()
        {
            try
            {
                var productos = context.Productos
                    .FromSqlRaw("EXEC sp_Producto_Listar")
                    .ToList();

                _event.RaiseSuccess("Productos obtenidos correctamente.");
                return productos.Select(p => new ProductoResponseDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    ImagenBase64 = p.Imagen != null
                        ? Convert.ToBase64String(p.Imagen)
                        : null
                });
            }
            catch (Exception ex)
            {
                _event.RaiseError(ex);
                return new List<ProductoResponseDTO>();
            }
        }


        public ProductoResponseDTO? GetById(int id)
        {
            try
            {
                var producto = context.Productos
                    .FromSqlRaw("EXEC sp_Producto_BuscarPorId @p0", id)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (producto == null)
                {
                    _event.RaiseFailed($"Producto con ID {id} no encontrado.");
                    return null;
                }


                _event.RaiseSuccess($"Producto {producto.Nombre} cargado correctamente.");
                return new ProductoResponseDTO
                {
                    Id = producto.Id,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Precio = producto.Precio,
                    ImagenBase64 = producto.Imagen != null
                        ? Convert.ToBase64String(producto.Imagen)
                        : null
                };
            }
            catch (Exception ex)
            {
                _event.RaiseError(ex);
                return null;
            }
        }


        public async Task Save(ProductoRequestDTO prod)
        {
            try
            {
                byte[] imagenBytes = null;

                if (prod.Imagen != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        await prod.Imagen.CopyToAsync(ms);
                        imagenBytes = ms.ToArray();
                    }
                }

                await context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Producto_Insertar @p0, @p1, @p2, @p3",
                    prod.Nombre,
                    prod.Descripcion,
                    imagenBytes,
                    prod.Precio
                );

                _event.RaiseSuccess($"Producto '{prod.Nombre}' guardado correctamente.");
            }
            catch (Exception ex)
            {
                _event.RaiseError(ex);
            }
        }



        public async Task Update(int id, ProductoRequestDTO prod)
        {
            try
            {
                if (prod.Id <= 0 || string.IsNullOrWhiteSpace(prod.Nombre) || prod.Precio <= 0)
                {
                    _event.RaiseError(new Exception("Nombre o precio inválido."));
                    return;
                }

                byte[] imagenBytes = null;

                if (prod.Imagen != null) // si subió una nueva imagen
                {
                    using var ms = new MemoryStream();
                    await prod.Imagen.CopyToAsync(ms);
                    imagenBytes = ms.ToArray();
                }

                await context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Producto_Modificar @p0, @p1, @p2, @p3, @p4",
                    prod.Id,
                    prod.Nombre,
                    prod.Descripcion,
                    (object)imagenBytes ?? DBNull.Value, // si no subió imagen, envías NULL
                    prod.Precio
                );

                _event.RaiseSuccess($"Producto '{prod.Nombre}' actualizado correctamente.");
            }
            catch (Exception ex)
            {
                _event.RaiseError(new Exception($"Error al actualizar producto: {ex.Message}", ex));
            }
        }


        public async Task Delete(int id)
        {
            try
            {
                await context.Database.ExecuteSqlRawAsync("EXEC sp_Producto_Eliminar @p0", id);
                _event.RaiseSuccess($"Producto con ID {id} eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _event.RaiseError(ex);
            }
        }
    }

    public interface IProductoService
    {
        IEnumerable<ProductoResponseDTO> Get();
        ProductoResponseDTO? GetById(int id);
        Task Save(ProductoRequestDTO prod);
        Task Update(int id, ProductoRequestDTO prod);
        Task Delete(int id);
    }
}
