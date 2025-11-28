using System.ComponentModel.DataAnnotations;

namespace PruebaTec.API.Models
{
    public class Venta
    {
        [Key]
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public DateTime Fecha { get; set; }
        public int IdUsuario { get; set; }
        public Usuario? Usuario { get; set; }
        public decimal Total { get; set; }
        public string? Estado { get; set; }

        public ICollection<DetalleVenta> DetallesVenta { get; set; }

    }
}
