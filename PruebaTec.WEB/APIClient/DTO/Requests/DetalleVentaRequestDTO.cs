namespace PruebaTec.WEB.APIClient.DTO.Requests
{
    public class DetalleVentaRequestDTO
    {
        public int IdVenta { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
