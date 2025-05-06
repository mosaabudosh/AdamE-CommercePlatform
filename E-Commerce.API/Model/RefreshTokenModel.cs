using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Model
{
    public class RefreshTokenModel
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
