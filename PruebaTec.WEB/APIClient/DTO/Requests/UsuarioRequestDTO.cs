namespace PruebaTec.WEB.APIClient.DTO.Requests
{
    public class UsuarioRequestDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Pwd { get; set; }
        public Rol Rol { get; set; }
    }

    public enum Rol
    {
        Administrador = 0,
        Operador = 1
    }
}
