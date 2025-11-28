using System.ComponentModel.DataAnnotations;

namespace PruebaTec.API.Models
{
    //response
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Pwd { get; set; }
        public int Rol { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Venta> Ventas { get; set; }
    }

}
