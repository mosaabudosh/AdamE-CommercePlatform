using E_Commerce.Domain.Domains.Authorisation;


namespace E_Commerce.Application.Authentication.Interfaces
{
    public interface ITokenHandler
    {
        UserLogin BuildTokens(UserLogin userLogin);

    }
}
