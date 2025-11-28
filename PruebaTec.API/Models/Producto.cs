using System.ComponentModel.DataAnnotations;

namespace PruebaTec.API.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public float Precio { get; set; }
        public byte[] Imagen { get; set; }

        public ICollection<DetalleVenta> DetallesVenta { get; set; }

    }
}
