using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TherapyAPI.TokenManager.Interfaces;

namespace TherapyAPI.TokenManager
{
    public class TokenManagerMiddleware : IMiddleware
    {
        private readonly ITokenManager TokenManager;

        public TokenManagerMiddleware(ITokenManager tokenManager)
        {
            TokenManager = tokenManager;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (await TokenManager.IsCurrentActiveToken())
            {
                await next(context);

                return;
            }
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
    }
}
