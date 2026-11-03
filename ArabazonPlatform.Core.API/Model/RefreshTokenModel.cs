using System.ComponentModel.DataAnnotations;

namespace ArabazonPlatform.API.Model
{
    public class RefreshTokenModel
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
