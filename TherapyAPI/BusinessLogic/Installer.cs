using System;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic
{
    public static class Installer
    {
        public static void AddBuisnessServices(this IServiceCollection container)
        {
            container
                .AddScoped<IUserService, UserService>()
                .AddScoped<IUserSessionService, UserSessionService>()
                .AddScoped<ISpecialistService, SpecialistService>()
                .AddScoped<IFileService, FileService>()
                .AddScoped<IProblemService, ProblemService>()
                .AddScoped<ISessionService, SessionService>()
                .AddScoped<IReviewService, ReviewService>();
        }
    }
}
