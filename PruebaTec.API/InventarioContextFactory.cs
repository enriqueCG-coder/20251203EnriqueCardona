using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace PruebaTec.API
{
    public class InventarioContextFactory : IDesignTimeDbContextFactory<InventarioContext>
    {
        public InventarioContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<InventarioContext>();

            // 🔹 Usa tu cadena de conexión local
            optionsBuilder.UseSqlServer("Data Source=");

            return new InventarioContext(optionsBuilder.Options);
        }
    }
}
