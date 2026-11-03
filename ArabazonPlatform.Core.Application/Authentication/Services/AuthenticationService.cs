

using ArabazonPlatform.Application.Authentication.Interfaces;

namespace ArabazonPlatform.Application.Authentication.Services
{
	 public class AuthenticationService : IAuthenticationService
     {

        public AuthenticationService()
        {
        }
        public void HashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            try
            {
                if (string.IsNullOrEmpty(password))
                {
                    throw new ArgumentNullException(nameof(password));
                }
                using (System.Security.Cryptography.HMACSHA512 HMACSHA512 = new System.Security.Cryptography.HMACSHA512())
                {
                    passwordSalt = HMACSHA512.Key;
                    passwordHash = HMACSHA512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            try
            {
                if (string.IsNullOrEmpty(password))
                {
                    throw new ArgumentNullException(nameof(password));
                }
                if (storedHash.Length != 64 || storedSalt.Length != 128)
                {
                    throw new ArgumentException("Invalid length of password.", nameof(password));
                }
                using (System.Security.Cryptography.HMACSHA512 HMACSHA512 = new System.Security.Cryptography.HMACSHA512(storedSalt))
                {
                    byte[] computedHash = HMACSHA512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != storedHash[i])
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
     }
}
