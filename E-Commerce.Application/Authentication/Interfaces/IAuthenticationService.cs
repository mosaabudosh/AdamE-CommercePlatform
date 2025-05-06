

using E_Commerce.Domain.Domains.Authorisation;

namespace E_Commerce.Application.Authentication.Interfaces
{
	public interface IAuthenticationService
	{
		void HashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt);
		bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);

    }
}
