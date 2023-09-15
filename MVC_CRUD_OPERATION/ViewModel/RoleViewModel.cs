using System.ComponentModel.DataAnnotations;

namespace MVC_CRUD_OPERATION.ViewModel
{
    public class RoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
