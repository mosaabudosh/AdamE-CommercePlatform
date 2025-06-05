using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Model
{
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
