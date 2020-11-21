using System;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using TherapyAPI.TokenManager.Interfaces;

namespace TherapyAPI.TokenManager
{
    public class TokenManager : ITokenManager
    {
        private readonly IDistributedCache Cache;
        private readonly IHttpContextAccessor HttpContextAccessor;
        private readonly IOptions<JwtOptions> JwtOptions;

        public TokenManager(IDistributedCache cache,
                IHttpContextAccessor httpContextAccessor,
                IOptions<JwtOptions> jwtOptions
            )
        {
            Cache = cache;
            HttpContextAccessor = httpContextAccessor;
            JwtOptions = jwtOptions;
        }

        public async Task<bool> IsCurrentActiveToken()
            => await IsActiveAsync(GetCurrentAsync());

        public async Task DeactivateCurrentAsync()
            => await DeactivateAsync(GetCurrentAsync());

        public async Task<bool> IsActiveAsync(string token)
            => await Cache.GetStringAsync(GetKey(token)) == null;

        public async Task DeactivateAsync(string token)
            => await Cache.SetStringAsync(GetKey(token),
                " ", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(JwtOptions.Value.ExpiryMinutes)
                });

        private string GetCurrentAsync()
        {
            var authorizationHeader = HttpContextAccessor
                .HttpContext.Request.Headers["authorization"];

            return authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Single().Split(" ").Last();
        }

        private static string GetKey(string token)
            => $"tokens:{token}:deactivated";
    }
}
