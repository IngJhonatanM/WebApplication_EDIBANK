using Microsoft.AspNetCore.Identity;

namespace EDIBANK.Models.Users_EdiWeb
{
    public class ApplicationUser : IdentityUser
    {
        public string? DescripUser { get; set; }

        public string? EDIId { get; set; }
    }
}