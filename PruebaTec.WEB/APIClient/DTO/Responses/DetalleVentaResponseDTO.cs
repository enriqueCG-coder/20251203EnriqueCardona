namespace PruebaTec.WEB.APIClient.DTO.Responses
{
    public class DetalleVentaResponseDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int IdVenta { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }

        public decimal Precio { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
    }
}
