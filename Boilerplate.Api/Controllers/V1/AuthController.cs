using System.Threading.Tasks;
using Boilerplate.Api.Utils;
using Boilerplate.Models;
using Boilerplate.Models.Auth;
using Boilerplate.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Api.Controllers.V1
{
    [ApiVersion("1.0"), Route("api/v{version:apiVersion}/auth")]
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <response code="422">Unprocessable Entity. When user with specified username already exist</response>
        [HttpPost("signUp")]
        [ProducesErrorResponse(StatusCodes.Status422UnprocessableEntity)]
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