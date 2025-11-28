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
    public class UsuarioController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UsuarioController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(string search, int? page)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7082/api/Usuario");

            if (!response.IsSuccessStatusCode)
            {
                TempData["AlertaTipo"] = "error";
                TempData["AlertaMensaje"] = "No se pudo cargar la lista de usuarios.";
                return View(new PagedList<UsuarioResponseDTO>(new List<UsuarioResponseDTO>(), 1, 10));
            }

            var json = await response.Content.ReadAsStringAsync();
            var usuarios = JsonConvert.DeserializeObject<List<UsuarioResponseDTO>>(json);

            if (!string.IsNullOrEmpty(search))
            {
                usuarios = usuarios
                    .Where(u => u.Nombre.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewData["Search"] = search;

            int pageNumber = page ?? 1;
            int pageSize = 10;

            var pagedUsuarios = usuarios.ToPagedList(pageNumber, pageSize);
            return View(pagedUsuarios);
        }


        public async Task<IActionResult> ToggleEstado(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PutAsync($"https://localhost:7082/api/Usuario/toggle/{id}", null);

            TempData["AlertaTipo"] = response.IsSuccessStatusCode ? "success" : "error";
            TempData["AlertaMensaje"] = response.IsSuccessStatusCode
                ? "Estado actualizado correctamente."
                : "No se pudo actualizar el estado.";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"https://localhost:7082/api/Usuario/{id}");

            TempData["AlertaTipo"] = response.IsSuccessStatusCode ? "success" : "error";
            TempData["AlertaMensaje"] = response.IsSuccessStatusCode
                ? "Usuario eliminado correctamente."
                : "No se pudo eliminar el usuario.";

            return RedirectToAction("Index");
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(UsuarioRequestDTO usuario)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:7082/api/Usuario", usuario);

            if (response.IsSuccessStatusCode)
            {
                TempData["AlertaTipo"] = "success";
                TempData["AlertaMensaje"] = "Usuario creado correctamente.";
                return RedirectToAction("Index");
            }

            TempData["AlertaTipo"] = "error";
            TempData["AlertaMensaje"] = "Error al crear el usuario.";
            return View(usuario);
        }


        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7082/api/Usuario/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["AlertaTipo"] = "error";
                TempData["AlertaMensaje"] = "No se pudo cargar el usuario.";
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            var usuario = JsonConvert.DeserializeObject<UsuarioRequestDTO>(json);

            return View(usuario);
        }


        [HttpPost]
        public async Task<IActionResult> Editar(UsuarioRequestDTO model)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"https://localhost:7082/api/Usuario/{model.Id}", content);

            TempData["AlertaTipo"] = response.IsSuccessStatusCode ? "success" : "error";
            TempData["AlertaMensaje"] = response.IsSuccessStatusCode
                ? "Usuario actualizado correctamente."
                : "Error al actualizar el usuario.";

            return response.IsSuccessStatusCode ? RedirectToAction("Index") : View(model);
        }

    }
}
