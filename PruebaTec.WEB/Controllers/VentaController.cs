using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PruebaTec.WEB.Filters;
using X.PagedList;
using PruebaTec.WEB.APIClient.DTO.Responses;
using X.PagedList.Extensions;

namespace PruebaTec.WEB.Controllers
{
    [AuthorizeToken]
    public class VentaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public VentaController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: Venta
        public async Task<IActionResult> Index(int? page)
        {
            var idUsuarioStr = HttpContext.Session.GetString("UsuarioId");

            if (!int.TryParse(idUsuarioStr, out int idUsuario))
            {
                TempData["AlertaTipo"] = "error";
                TempData["AlertaMensaje"] = "Usuario no autenticado.";
                return RedirectToAction("Login", "Login");
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7082/api/Venta/usuario/{idUsuario}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["AlertaTipo"] = "error";
                TempData["AlertaMensaje"] = "No se pudo cargar la lista de ventas.";

                return View(new PagedList<VentaResponseDTO>(new List<VentaResponseDTO>(), 1, 10));
            }

            var json = await response.Content.ReadAsStringAsync();
            var ventas = JsonConvert.DeserializeObject<List<VentaResponseDTO>>(json);

            int pageNumber = page ?? 1;
            int pageSize = 10;

            return View(ventas.ToPagedList(pageNumber, pageSize));
        }


        // POST: Venta/Crear
        [HttpPost]
        public async Task<IActionResult> Crear()
        {
            try
            {
                var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");

                if (!int.TryParse(usuarioIdStr, out int usuarioId))
                {
                    TempData["AlertaTipo"] = "error";
                    TempData["AlertaMensaje"] = "No se encontró el usuario en la sesión.";
                    return RedirectToAction("Index");
                }

                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsync($"https://localhost:7082/api/Venta/{usuarioId}", null);

                TempData["AlertaTipo"] =
                    response.IsSuccessStatusCode ? "success" : "error";

                TempData["AlertaMensaje"] =
                    response.IsSuccessStatusCode ? "Venta registrada correctamente."
                                                 : "Error al registrar la venta.";
            }
            catch (Exception ex)
            {
                TempData["AlertaTipo"] = "error";
                TempData["AlertaMensaje"] = $"Error: {ex.Message}";
            }

            return RedirectToAction("Index");
        }


        // GET: Venta/Detalle/5
        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var idUsuarioStr = HttpContext.Session.GetString("UsuarioId");
            if (!int.TryParse(idUsuarioStr, out int idUsuario))
            {
                TempData["AlertaTipo"] = "error";
                TempData["AlertaMensaje"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Login");
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7082/api/Venta/{id}/{idUsuario}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["AlertaTipo"] = "error";
                TempData["AlertaMensaje"] = "No se pudo cargar la venta.";
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            var venta = JsonConvert.DeserializeObject<VentaResponseDTO>(json);

            return View(venta);
        }


        // GET: Venta/Eliminar/5
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"https://localhost:7082/api/Venta/{id}");

            TempData["AlertaTipo"] = response.IsSuccessStatusCode ? "success" : "error";
            TempData["AlertaMensaje"] = response.IsSuccessStatusCode
                ? "Venta eliminada correctamente."
                : "No se pudo eliminar la venta.";

            return RedirectToAction("Index");
        }


        // POST: Venta/Confirmar
        [HttpPost]
        public async Task<IActionResult> Confirmar(int idVenta)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(
                $"https://localhost:7082/api/Venta/confirmar/{idVenta}", null
            );

            TempData["AlertaTipo"] = response.IsSuccessStatusCode ? "success" : "error";
            TempData["AlertaMensaje"] = response.IsSuccessStatusCode
                ? "Venta confirmada correctamente."
                : "No se pudo confirmar la venta.";

            return RedirectToAction("Index");
        }
    }
}
