using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PruebaTec.WEB.Filters;
using PruebaTec.WEB.APIClient.DTO;
using System.Net.Http;
using System.Text;
using X.PagedList;
using PruebaTec.WEB.APIClient.DTO.Responses;
using PruebaTec.WEB.APIClient.DTO.Requests;

namespace PruebaTec.WEB.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public async Task<IActionResult> Login(LoginRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["AlertaTipo"] = "error";
                TempData["AlertaMensaje"] = "Datos inválidos.";
                RedirectToAction("Index", "Login");
            }

            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7082/api/Login/login", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["AlertaTipo"] = "error";
                TempData["AlertaMensaje"] = "Usuario o contraseña incorrectos.";
                return RedirectToAction("Index", "Login");
            }

            var result = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<LoginResponseDTO>(result);

            HttpContext.Session.SetString("JWToken", data.Token);
            HttpContext.Session.SetString("UsuarioId", data.Usuario.Id.ToString());
            HttpContext.Session.SetString("UsuarioNombre", data.Usuario.Nombre);
            HttpContext.Session.SetString("UsuarioRol", data.Usuario.Rol.ToString());


            TempData["AlertaTipo"] = "success";
            TempData["AlertaMensaje"] = "Bienvenido, " + data.Usuario.Nombre;
            return View("LoginSuccess");

        }
    }
}
