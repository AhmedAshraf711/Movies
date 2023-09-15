using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MVC_CRUD_OPERATION.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string Address { get; set; }
    }
}
