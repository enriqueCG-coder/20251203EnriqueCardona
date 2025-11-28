namespace PruebaTec.WEB.APIClient.DTO.Requests
{
    public class ProductoRequestDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public float Precio { get; set; }
        public IFormFile Imagen { get; set; }
    }
}
