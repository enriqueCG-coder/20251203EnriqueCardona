using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PruebaTec.WEB.Filters;
using PruebaTec.WEB.APIClient.DTO;
using System.Text;
using X.PagedList;
using X.PagedList.Extensions;
using PruebaTec.WEB.APIClient.DTO.Responses;
using PruebaTec.WEB.APIClient.DTO.Requests;

namespace PruebaTec.WEB.Controllers
{
    [AuthorizeToken]
    public class DetalleVentaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DetalleVentaController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: DetalleVenta/Index/5
        public async Task<IActionResult> Index(int idVenta, int? page)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7082/api/DetalleVenta/venta/{idVenta}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo cargar el detalle de la venta.";
                return View(new PagedList<DetalleVentaResponseDTO>(new List<DetalleVentaResponseDTO>(), 1, 10));
            }

            var json = await response.Content.ReadAsStringAsync();
            var detalles = JsonConvert.DeserializeObject<List<DetalleVentaResponseDTO>>(json);

            int pageNumber = page ?? 1;
            int pageSize = 10;

            ViewBag.IdVenta = idVenta;
            return View(detalles.ToPagedList(pageNumber, pageSize));
        }


        // POST: DetalleVenta/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DetalleVentaRequestDTO request)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.PostAsJsonAsync(
                "https://localhost:7082/api/DetalleVenta",
                request
            );

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Detalle registrado correctamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo registrar el detalle.";
            }

            return RedirectToAction("Index", new { idVenta = request.IdVenta });
        }


        // POST: DetalleVenta/Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id, int idVenta)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"https://localhost:7082/api/DetalleVenta/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["AlertaTipo"] = "success";
                TempData["AlertaMensaje"] = "Detalle eliminado correctamente.";
            }
            else
            {
                TempData["AlertaTipo"] = "error";
                TempData["AlertaMensaje"] = "No se pudo eliminar el detalle.";
            }

            return RedirectToAction("Index", new { idVenta = idVenta });
        }
    }
}
