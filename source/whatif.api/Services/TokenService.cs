using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WhatIf.Api.Services
{    public interface ITokenService
    {
        string BuildToken(Guid userId, string email);
    }
    public class TokenService : ITokenService
    {
        private TimeSpan ExpiryDuration = new TimeSpan(12, 30, 0);
        private string issuer;
        private string key;
        public TokenService(string key, string issuer)
        {
            System.Console.WriteLine($"================================{key}");
            this.issuer = issuer;
            this.key = key;
        }

        public string BuildToken(Guid userId, string email)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, "WalletUser")
            };
    
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
                expires: DateTime.Now.Add(ExpiryDuration), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}