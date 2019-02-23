using System.Threading.Tasks;
using Boilerplate.Models;
using Boilerplate.Models.Auth;
using Boilerplate.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Api.Controllers
{
    [ApiVersion("1.0"), Route("api/v{version:apiVersion}/auth")]
    public class AuthV1Controller : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthV1Controller(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Sign up user with login and password
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Authorization data with bearer token</returns>
        [HttpPost("signUp")]
        [ProducesResponseType(typeof(BaseResponse<ErrorModel>), StatusCodes.Status400BadRequest)]
        public async Task<BaseResponse<AuthResultModel>> SignUp(SignUpModel model) =>
            await PrepareResponse(_authService.RegisterUser, model);

        /// <summary>
        /// Sign in user with login and password
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Authorization data with bearer token</returns>
        [HttpPost("signIn")]
        [ProducesResponseType(typeof(BaseResponse<ErrorModel>), StatusCodes.Status401Unauthorized)]
        public async Task<BaseResponse<AuthResultModel>> SignIn(SignInModel model) =>
            await PrepareResponse(_authService.AuthorizeUser, model);

        /// <summary>
        /// Refresh user's authorization token with refresh token
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Authorization data with bearer token</returns>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(BaseResponse<ErrorModel>), StatusCodes.Status401Unauthorized)]
        public async Task<BaseResponse<AuthResultModel>> Refresh(RefreshRequestModel model) =>
            await PrepareResponse(_authService.RefreshUserToken, model);
    }
}