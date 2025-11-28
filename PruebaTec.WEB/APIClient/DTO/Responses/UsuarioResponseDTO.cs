namespace PruebaTec.WEB.APIClient.DTO.Responses
{
    public class UsuarioResponseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public Rol Rol { get; set; }
        public bool IsActive { get; set; }
    }

    public enum Rol
    {
        Administrador = 0,
        Operador = 1
    }

}
