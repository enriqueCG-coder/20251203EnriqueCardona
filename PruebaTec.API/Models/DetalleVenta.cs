using System.Text.Json.Serialization;

namespace PruebaTec.API.Models
{
    public class DetalleVenta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int IdVenta { get; set; }
        public int IdProducto { get; set; }

        [JsonIgnore]
        public Venta Venta { get; set; }

        public Producto Producto { get; set; }

        public int Cantidad { get; set; }

        public decimal Precio { get; set; }
        public decimal Iva { get; set; }

        public decimal Total { get; set; }

    }
}
