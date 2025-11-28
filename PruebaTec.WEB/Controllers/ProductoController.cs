using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PruebaTec.WEB.Filters;
using PruebaTec.WEB.APIClient.DTO;
using System.Net.Http;
using System.Text;
using X.PagedList;
using X.PagedList.Extensions;
using PruebaTec.WEB.APIClient.DTO.Responses;
using PruebaTec.WEB.APIClient.DTO.Requests;

namespace PruebaTec.WEB.Controllers
{
    [AuthorizeToken]
    public class ProductoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(string search, int? page)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7082/api/Producto");

            if (!response.IsSuccessStatusCode)
            {
                TempData["AlertaTipo"] = "error";
                TempData["AlertaMensaje"] = "No se pudo cargar la lista de productos.";
                return View(new PagedList<ProductoResponseDTO>(new List<ProductoResponseDTO>(), 1, 10));
            }

            var json = await response.Content.ReadAsStringAsync();
            var productos = JsonConvert.DeserializeObject<List<ProductoResponseDTO>>(json);

            if (!string.IsNullOrEmpty(search))
            {
                productos = productos
                    .Where(u => u.Nombre.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewData["Search"] = search;

            int pageNumber = page ?? 1;
            int pageSize = 10;

            var pagedProductos = productos.ToPagedList(pageNumber, pageSize);
            return View(pagedProductos);
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"https://localhost:7082/api/Producto/{id}");

            TempData["AlertaTipo"] = response.IsSuccessStatusCode ? "success" : "error";
            TempData["AlertaMensaje"] = response.IsSuccessStatusCode
                ? "Producto eliminado correctamente."
                : "No se pudo eliminar el producto.";

            return RedirectToAction("Index");
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(ProductoRequestDTO prod)
        {
            var client = _httpClientFactory.CreateClient();

            using var form = new MultipartFormDataContent();

            form.Add(new StringContent(prod.Nombre), "Nombre");
            form.Add(new StringContent(prod.Descripcion), "Descripcion");
            form.Add(new StringContent(
    prod.Precio.ToString(System.Globalization.CultureInfo.InvariantCulture)),
    "Precio"
);


            // Imagen
            if (prod.Imagen != null)
            {
                var streamContent = new StreamContent(prod.Imagen.OpenReadStream());
                form.Add(streamContent, "Imagen", prod.Imagen.FileName);
            }

            var response = await client.PostAsync("https://localhost:7082/api/Producto", form);

            if (response.IsSuccessStatusCode)
            {
                TempData["AlertaTipo"] = "success";
                TempData["AlertaMensaje"] = "Producto creado correctamente.";
                return RedirectToAction("Index");
            }

            TempData["AlertaTipo"] = "error";
            TempData["AlertaMensaje"] = "Error al crear el producto.";
            return View(prod);
        }


        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7082/api/Producto/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["AlertaTipo"] = "error";
                TempData["AlertaMensaje"] = "No se pudo cargar el producto.";
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            var prod = JsonConvert.DeserializeObject<ProductoResponseDTO>(json);

            return View(prod);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(ProductoRequestDTO model)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"https://localhost:7082/api/Producto/{model.Id}", content);

            TempData["AlertaTipo"] = response.IsSuccessStatusCode ? "success" : "error";
            TempData["AlertaMensaje"] = response.IsSuccessStatusCode
                ? "Producto actualizado correctamente."
                : "Error al actualizar el usuario.";

            return response.IsSuccessStatusCode ? RedirectToAction("Index") : View(model);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7082/api/Producto/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var producto = JsonConvert.DeserializeObject<ProductoRequestDTO>(json);

            return Json(new { nombre = producto.Nombre, precio = producto.Precio });
        }
    }
}
