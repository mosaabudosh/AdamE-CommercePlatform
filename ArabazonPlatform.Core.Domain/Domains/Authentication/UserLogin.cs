

namespace ArabazonPlatform.Domain.Domains.Authorisation
{
    public class UserLogin : SharedBase
    {
        public Guid UserLoginId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public long RefreshTokenExpirationDate { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime? LogOutDate { get; set; }
        public Guid UserId { get; set; }

        public virtual User User { get; set; }
    }
}
