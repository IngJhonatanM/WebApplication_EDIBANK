using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EDIBANK.Models.Users_EdiWeb
{
    public class Users
    {
        [Display(Name = "Nombre de usuario")]
        [Required(ErrorMessage = "Por favor, ingresar nombre. ")]
        public required string Name { get; init; }

        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "Por favor, ingresar email. ")]
        public string? Email { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Por favor, ingresar Contraseña. ")]
        public required string Password { get; init; }

        [Display(Name = "Descripción de usuario")]
        public string? DescripUser { get; set; }

        public string? SelectedRole { get; set; }
        public List<SelectListItem>? Roles { get; set; }

        public string? EDIId { get; init; }

        [Display(Name = "EDI asociado")]
        public SelectList? EDIs { get; set; }
    }
}