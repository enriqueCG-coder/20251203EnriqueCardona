using Microsoft.EntityFrameworkCore;
using PruebaTec.API.DTO;
using PruebaTec.API.DTO.Responses;
using PruebaTec.API.Models;
using System.Linq.Expressions;
using System.Threading;

namespace PruebaTec.API
{
    public class InventarioContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> Detalles { get; set; }
        public DbSet<VentaResponseDTO> GetVentas { get; set; }
        public DbSet<DetalleVentaResponseDTO> GetDetalles { get; set; }

        public InventarioContext(DbContextOptions<InventarioContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VentaResponseDTO>().HasNoKey(); 
            base.OnModelCreating(modelBuilder);

            var usuarioInit = new List<Usuario>
            {
                new Usuario
                {
                    Id = 1,
                    Nombre = "admin",
                    //admin
                    Pwd = "$2a$11$GXYXICmOBbheKvV8Qe6IzuOFBUhQqGcaVqddIexWNpHNcFfHIhJRm",
                    Rol = 0,
                    IsActive = true
                }
            };

            modelBuilder.Entity<Usuario>(user =>
            {
                user.ToTable("Usuario");
                user.HasKey(p => p.Id);
                user.Property(p => p.Id).ValueGeneratedOnAdd();
                user.Property(p => p.Nombre).IsRequired().HasMaxLength(150);
                user.Property(p => p.Pwd).IsRequired();
                user.Property(p => p.IsActive).IsRequired();
                user.HasData(usuarioInit);
            });


            modelBuilder.Entity<Producto>(prd =>
            {
                prd.ToTable("Producto");
                prd.HasKey(p => p.Id);
                prd.Property(p => p.Id)
                   .UseIdentityColumn(seed: 1000, increment: 1);

                prd.Property(p => p.Nombre).IsRequired().HasMaxLength(150);
                prd.Property(p => p.Descripcion).IsRequired().HasMaxLength(150);
                prd.Property(p => p.Imagen).HasColumnType("varbinary(max)").IsRequired(false);
                prd.Property(p => p.Precio).IsRequired().HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Venta>(venta =>
            {
                venta.ToTable("Venta");

                venta.HasKey(p => p.Id);

                venta.Property(p => p.Id)
                     .UseIdentityColumn(seed: 1000, increment: 1);
                venta.Property(p => p.Codigo).IsRequired().HasMaxLength(36);

                venta.Property(p => p.Fecha)
                     .IsRequired();

                venta.Property(p => p.Total)
                     .IsRequired().HasColumnType("decimal(18,2)");

                venta.Property(p => p.Estado).IsRequired().HasMaxLength(25);
                // 🔹 Relación con Usuario (1 usuario -> muchas ventas)
                venta.HasOne(p => p.Usuario)
                     .WithMany(u => u.Ventas)
                     .HasForeignKey(p => p.IdUsuario)
                     .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DetalleVenta>(detalle =>
            {
                detalle.ToTable("DetalleVenta");
                detalle.HasKey(p => p.Id);

                detalle.Property(p => p.Id)
                       .UseIdentityColumn(seed: 1000, increment: 1);

                detalle.Property(p => p.Fecha)
                       .IsRequired();

                detalle.Property(p => p.Cantidad)
                       .IsRequired();

                detalle.Property(p => p.Precio)
                       .HasColumnType("decimal(18,2)");

                detalle.Property(p => p.Iva)
                       .HasColumnType("decimal(18,2)");

                detalle.Property(p => p.Total)
                       .HasColumnType("decimal(18,2)");

                // 🔸 Relación con Venta
                detalle.HasOne(d => d.Venta)
                       .WithMany(v => v.DetallesVenta)
                       .HasForeignKey(d => d.IdVenta)
                       .OnDelete(DeleteBehavior.Cascade);
                // si eliminas la venta, se eliminan sus detalles

                // 🔸 Relación con Producto
                detalle.HasOne(d => d.Producto)
                       .WithMany(p => p.DetallesVenta)
                       .HasForeignKey(d => d.IdProducto)
                       .OnDelete(DeleteBehavior.Restrict);
                // no permite eliminar producto si tiene detalles
            });


        }
    }
}
