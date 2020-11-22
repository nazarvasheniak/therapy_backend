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
                .AddScoped<IUserWalletService, UserWalletService>()
                .AddScoped<ISpecialistService, SpecialistService>()
                .AddScoped<IFileService, FileService>()
                .AddScoped<IProblemService, ProblemService>()
                .AddScoped<IProblemImageService, ProblemImageService>()
                .AddScoped<IProblemResourceService, ProblemResourceService>()
                .AddScoped<IProblemResourceTaskService, ProblemResourceTaskService>()
                .AddScoped<ISessionService, SessionService>()
                .AddScoped<IReviewService, ReviewService>()
                .AddScoped<IArticleService, ArticleService>()
                .AddScoped<IArticleLikeService, ArticleLikeService>()
                .AddScoped<IArticleCommentService, ArticleCommentService>()
                .AddScoped<IArticlePublishService, ArticlePublishService>()
                .AddScoped<IPaymentService, PaymentService>()
                .AddScoped<IUserVerificationService, UserVerificationService>()
                .AddScoped<IUserVerificationRequestService, UserVerificationRequestService>()
                .AddScoped<IClientVideoReviewService, ClientVideoReviewService>();
        }
    }
}
