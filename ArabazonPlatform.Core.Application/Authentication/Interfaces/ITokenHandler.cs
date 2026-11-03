using ArabazonPlatform.Domain.Domains.Authentication;
using ArabazonPlatform.Domain.Domains.Authorisation;


namespace ArabazonPlatform.Application.Authentication.Interfaces
{
    public interface ITokenHandler
    {
        UserLogin BuildUserTokens(UserLogin userLogin);
        CustomerLogin BuildCustomerTokens(CustomerLogin customerLogin);
    }
}
