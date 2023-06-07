using AuthorizationServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly List<Client> clients = new List<Client>
        {
            new Client { ClientId = "client1", ClientSecret = "secret1" },
            new Client { ClientId = "client2", ClientSecret = "secret2" }
        };

        private readonly List<User> users = new List<User>
        {
            new User { Username = "user1", Password = "password1" },
            new User { Username = "user2", Password = "password2" },
            new User { Username = "user3", Password = "password3" },
        };

        [HttpPost("GetAccessToken")]
        public IActionResult GetAccessToken([FromBody] TokenRequest tokenRequest)
        {
            // Authentication request validation
            if (IsValidClient(tokenRequest.ClientId, tokenRequest.ClientSecret) &&
                IsValidUser(tokenRequest.Username, tokenRequest.Password))
            {
                // Generating and returning an access token
                var accessToken = GenerateAccessToken(tokenRequest.ClientId);
                return Ok(new { AccessToken = accessToken });
            }

            return Unauthorized();
        }

        private bool IsValidClient(string clientId, string clientSecret)
        {
            return clients.Any(c => c.ClientId == clientId && c.ClientSecret == clientSecret);
        }

        private bool IsValidUser(string username, string password)
        {
            return true;// users.Any(u => u.Username == username && u.Password == password);
        }

        private string GenerateAccessToken(string clientId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("HereIsTheSecretKeyForJWTBearerForAuthenticationClient"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> { };

            if (clientId == "client1")
            {
                claims.Add(new Claim(ClaimTypes.Role, "GetWeather"));
                //claims = new List<Claim>
                //{
                //    //new Claim(ClaimTypes.Name, "user1"),
                //    new Claim(ClaimTypes.Role, "GetWeather") // Add claim with information about user role.
                //};
            }

            var token = new JwtSecurityToken(
                //issuer: "AuthServer",
                //audience: "Client", //
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials,
                claims: claims
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var accessToken = tokenHandler.WriteToken(token);

            return accessToken;
        }
    }
}
