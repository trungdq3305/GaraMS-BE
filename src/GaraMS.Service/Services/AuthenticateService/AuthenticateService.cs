using GaraMS.Data.ViewModels.AutheticateModel;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GaraMS.Service.Services.AutheticateService
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly JwtSecurityTokenHandler _jwtHandler;
        private readonly IConfiguration _config;
        public AuthenticateService(IConfiguration config)
        {
            _config = config;
            _jwtHandler = new JwtSecurityTokenHandler();
        }
        public string decodeToken(string jwtToken, string nameClaim)
        {
            Claim? claim = _jwtHandler.ReadJwtToken(jwtToken).Claims.FirstOrDefault(selector => selector.Type.ToString().Equals(nameClaim));
            return claim != null ? claim.Value : "Error!!!";
        }

        public string GenerateJWT(LoginResModel User)
        {
            // Retrieve JWT Key
            string? jwtKey = _config["JwtSettings:JwtKey"];
            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is missing in configuration.");
            }

            // Convert string key to byte array
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create claims
            var claims = new List<Claim>
    {
        new Claim(ClaimsIdentity.DefaultRoleClaimType, User.Role.ToString()),
        new Claim("userid", User.UserId.ToString())
,
    };

            // Generate JWT token
            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
