using System.ComponentModel.DataAnnotations;

namespace PruebaTec.WEB.APIClient.DTO.Requests
{
    public class LoginRequestDTO
    {
        [Required]
        [Display(Name = "Usuario")]
        public string Nombre { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}
