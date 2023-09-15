using System.ComponentModel.DataAnnotations;

namespace MVC_CRUD_OPERATION.ViewModel
{
    public class Registerviwemodel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string Address { get; set; }
    }
}
