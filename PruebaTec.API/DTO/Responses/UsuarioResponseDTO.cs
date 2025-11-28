namespace PruebaTec.API.DTO.Responses
{
    public class UsuarioResponseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Rol { get; set; }
        public bool IsActive { get; set; }
    }
}
