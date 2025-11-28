namespace PruebaTec.WEB.APIClient.DTO.Responses
{
    public class ProductoResponseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public float Precio { get; set; }
        public IFormFile Imagen { get; set; }
        public string ImagenBase64 { get; set; }
    }
}
