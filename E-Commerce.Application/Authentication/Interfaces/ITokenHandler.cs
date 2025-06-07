using E_Commerce.Domain.Domains.Authentication;
using E_Commerce.Domain.Domains.Authorisation;
using Generic.Response;


namespace E_Commerce.Application.Authentication.Interfaces
{
    public interface ITokenHandler
    {
        UserLogin BuildUserTokens(UserLogin userLogin);
        CustomerLogin BuildCustomerTokens(CustomerLogin customerLogin);
    }
}
