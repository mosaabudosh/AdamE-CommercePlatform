

using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace E_Commerce.Application.Authentication.Security
{
    public class SigningCredentialsConfiguration
    {
        public SecurityKey Key { get; }
        public SigningCredentials SigningCredentials { get; }

        public SigningCredentialsConfiguration()
        {
            Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("THIS IS USED TO SIGN AND VERIFY JWT TOKENS, REPLACE IT WITH YOUR OWN SECRET, IT CAN BE ANY STRING"));
            SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256Signature);
        }
    }
}
