using System;
using System.Threading.Tasks;
using Boilerplate.DAL.Entities;
using Boilerplate.Models.Auth;

namespace Boilerplate.Services.Abstractions
{
    public interface IAuthService
    {
        Task<AuthResultModel> RegisterUser(SignUpModel model);
        Task<AuthResultModel> AuthorizeUser(SignInModel model);
        Task<AuthResultModel> RefreshUserToken(RefreshRequestModel refreshRequest);
        bool IsAuthorized();
        Guid GetAuthorizedUserId();
        Task<ApplicationUser> GetAuthorizedUser();
    }
}
