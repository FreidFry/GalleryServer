﻿using dotenv.net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Gallery.Server.Infrastructure.Persistence.Models;
using Gallery.Server.Core.Interfaces;


namespace Gallery.Server.Core.Services
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _options;

        public JwtProvider(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateToken(UserModel user)
        {
            Claim[] claims = [new("uid", user.UserId.ToString())];

            var singningKey = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256
                );

            var token = new JwtSecurityToken(
                issuer: _options.issuer,
                audience: _options.audience,
                claims: claims,
                signingCredentials: singningKey,
                expires: DateTime.UtcNow.AddDays(_options.ExpiresDays)
                );

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenValue;
        }

    }
}
