

using ArabazonPlatform.Generic.Models;
using ArabazonPlatform.Application.Authentication.Interfaces;
using ArabazonPlatform.Domain.Domains.Authentication;
using ArabazonPlatform.Domain.Domains.Authorisation;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ArabazonPlatform.Application.Authentication.Security
{
    public class TokenHandler : ITokenHandler
    {
        #region Fields
        private readonly TokenOptions _tokenOptions;
        private readonly SigningCredentialsConfiguration _signingConfigurations;
        private readonly IConfiguration _configuration;
        #endregion

        public TokenHandler(IOptions<TokenOptions> tokenOptions, SigningCredentialsConfiguration signingConfigurations, IConfiguration configuration)
        {
            _tokenOptions = tokenOptions.Value;
            _signingConfigurations = signingConfigurations;
            _configuration = configuration;
        }
        public UserLogin BuildUserTokens(UserLogin userLogin)
        {
            try
            {
                int refreshTokenExpirationMinutes = 5;
                int accessTokenValidDays = 1;
                int tokenSize = 256;

                if (_configuration.GetSection("AuthenticationOption").Exists())
                {
                    refreshTokenExpirationMinutes = _configuration.GetSection("AuthenticationOption").GetSection("RefreshTokenExpirationMinutes").Exists() ? Convert.ToInt32(_configuration.GetSection("AuthenticationOption").GetSection("RefreshTokenExpirationMinutes").Value) : refreshTokenExpirationMinutes;
                    accessTokenValidDays = _configuration.GetSection("AuthenticationOption").GetSection("AccessTokenValidDays").Exists() ? Convert.ToInt32(_configuration.GetSection("AuthenticationOption").GetSection("AccessTokenValidDays").Value) : accessTokenValidDays;
                    tokenSize = _configuration.GetSection("AuthenticationOption").GetSection("RefreshTokenSizeByt").Exists() ? Convert.ToInt32(_configuration.GetSection("AuthenticationOption").GetSection("RefreshTokenSizeByt").Value) : tokenSize;
                }
                userLogin.RefreshTokenExpirationDate = DateTime.Now.AddMinutes(refreshTokenExpirationMinutes).Ticks;
                userLogin.AccessToken = GenerateToken(userLogin.User,null, DateTime.Now.AddDays(accessTokenValidDays));
                userLogin.RefreshToken = GenerateToken(userLogin.User,null, DateTime.Now.AddMinutes(refreshTokenExpirationMinutes), true);
                return userLogin;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public CustomerLogin BuildCustomerTokens(CustomerLogin customerLogin)
        {
            try
            {
                int refreshTokenExpirationMinutes = 5;
                int accessTokenValidDays = 1;
                int tokenSize = 256;

                if (_configuration.GetSection("AuthenticationOption").Exists())
                {
                    refreshTokenExpirationMinutes = _configuration.GetSection("AuthenticationOption").GetSection("RefreshTokenExpirationMinutes").Exists() ? Convert.ToInt32(_configuration.GetSection("AuthenticationOption").GetSection("RefreshTokenExpirationMinutes").Value) : refreshTokenExpirationMinutes;
                    accessTokenValidDays = _configuration.GetSection("AuthenticationOption").GetSection("AccessTokenValidDays").Exists() ? Convert.ToInt32(_configuration.GetSection("AuthenticationOption").GetSection("AccessTokenValidDays").Value) : accessTokenValidDays;
                    tokenSize = _configuration.GetSection("AuthenticationOption").GetSection("RefreshTokenSizeByt").Exists() ? Convert.ToInt32(_configuration.GetSection("AuthenticationOption").GetSection("RefreshTokenSizeByt").Value) : tokenSize;
                }
                customerLogin.RefreshTokenExpirationDate = DateTime.Now.AddMinutes(refreshTokenExpirationMinutes).Ticks;
                customerLogin.AccessToken = GenerateToken(null,customerLogin.Customer, DateTime.Now.AddDays(accessTokenValidDays));
                customerLogin.RefreshToken = GenerateToken(null,customerLogin.Customer, DateTime.Now.AddMinutes(refreshTokenExpirationMinutes), true);
                return customerLogin;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GenerateToken(User? user, Customer? customer, DateTime tokenExpiration, bool isRefresh = false)
        {
            try
            {
                List<Claim> claims = new List<Claim>();
                if (isRefresh)
                {
                    claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.UniqueName, tokenExpiration.ToString())
                    };
                }
                else
                {
                    if(user is not null)
                    {
                        claims = new List<Claim>
                        {
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),

                        };
                        if (user.UserRoles != null && user.UserRoles.Any(p => p.Role != null))
                        {
                            claims.AddRange(user.UserRoles.Where(p => p.Role != null).Select(s => new Claim(ClaimTypes.Role, s.Role.Name)).ToList());
                        }
                    }
                    if (customer is not null)
                    {
                        claims = new List<Claim>
                        {
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(ClaimTypes.NameIdentifier, customer.CustomerId.ToString()),
                            new Claim(ClaimTypes.Name, $"{customer.FirstName} {customer.LastName}"),
                            new Claim(JwtRegisteredClaimNames.Sub, customer.CustomerId.ToString()),

                        };
                    }

                }
                SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
                {
                    Expires = tokenExpiration,
                    IssuedAt = DateTime.Now,
                    Subject = new ClaimsIdentity(claims),
                    Issuer = _tokenOptions.Issuer,
                    Audience = _tokenOptions.Audience,
                    SigningCredentials = _signingConfigurations.SigningCredentials
                };
                JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
                return jwtSecurityTokenHandler.WriteToken(securityToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
 
    }
}
