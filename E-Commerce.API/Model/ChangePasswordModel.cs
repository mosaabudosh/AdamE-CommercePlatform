using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Model
{
    public class ChangePasswordModel
    {
        public Guid UserId { get; set; }
        public string OldPassword { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
