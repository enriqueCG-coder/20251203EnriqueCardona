using System.ComponentModel.DataAnnotations;

namespace PruebaTec.API.DTO.Requests
{
    public class UsuarioRequestDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Pwd { get; set; }   // Password sin hash (solo en request)
        public RolDTO Rol { get; set; }      // Solo cuando sea registro, no en login
    }

}
