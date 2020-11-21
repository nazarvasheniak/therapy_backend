using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using BusinessLogic.Interfaces;
using Domain.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Storage.Interfaces;
using BusinessLogic.Config;
using System.Text;

namespace BusinessLogic.Services
{
    public class UserSessionService : BaseCrudService<UserSession>, IUserSessionService
    {
        private IConfiguration Configuration { get; set; }

        public UserSessionService(IRepository<UserSession> repository,
            IConfiguration configuration) : base(repository)
        {
            Configuration = configuration;
        }

        public UserSession CreateSession(User user)
        {
            var session = new UserSession
            {
                User = user,
                AuthCode = RandomNumber().ToString(),
                Created = DateTime.UtcNow,
                IsActive = true
            };

            Create(session);

            return session;
        }

        public UserSession GetUserActiveSession(User user)
        {
            return GetAll().FirstOrDefault(x => x.User == user && x.IsActive);
        }

        public void CloseUserActiveSession(User user)
        {
            var session = GetAll().FirstOrDefault(x => x.User == user && x.IsActive);
            if (session != null)
            {
                session.IsActive = false;
                Update(session);
            }
        }

        public string AuthorizeUser(User user)
        {
            var token = GetSecurityToken(GetIdentity(user));
            SaveToken(user, token);

            return token;
        }

        private ClaimsIdentity GetIdentity(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.ID.ToString()),
                new Claim("role", user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                user.Role.ToString());

            return claimsIdentity;
        }

        private string GetSecurityToken(ClaimsIdentity identity)
        {
            var jwtSection = Configuration.GetSection("jwt");
            var jwtOptions = new JwtOptions
            {
                ExpiryMinutes = int.Parse(jwtSection["expiryMinutes"]),
                SecretKey = jwtSection["secretKey"],
                Issuer = jwtSection["issuer"]
            };

            var now = DateTime.UtcNow;
            var expires = now.Add(TimeSpan.FromMinutes(jwtOptions.ExpiryMinutes));

            var jwt = new JwtSecurityToken(
                    issuer: jwtOptions.Issuer,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: expires,
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                        SecurityAlgorithms.HmacSha256)
                    );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        private void SaveToken(User user, string token)
        {
            var session = GetUserActiveSession(user);
            session.Token = token;

            Update(session);
        }

        private int RandomNumber()
        {
            Random random = new Random();
            return random.Next(1000, 9999);
        }
    }
}
