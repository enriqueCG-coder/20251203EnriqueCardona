namespace PruebaTec.WEB.APIClient.DTO.Responses
{
    public class VentaResponseDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public DateTime Fecha { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
    }
}
