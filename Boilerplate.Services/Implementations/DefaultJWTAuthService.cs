using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Boilerplate.DAL;
using Boilerplate.DAL.Entities;
using Boilerplate.Models.Auth;
using Boilerplate.Models.Exceptions;
using Boilerplate.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Boilerplate.Services.Implementations
{
    public class DefaultJwtAuthService: BaseDataService, IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public DefaultJwtAuthService(IConfiguration configuration, ApplicationDbContext context,
            IHttpContextAccessor contextAccessor, UserManager<ApplicationUser> userManager, IMapper mapper) : base(context, mapper)
        {
            _configuration = configuration;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        public async Task<AuthResultModel> RegisterUser(SignUpModel model)
        {
            if (await DbContext.Users.FirstOrDefaultAsync(_ => _.UserName == model.Username) != null)
                throw new BadRequestException("User with this name already exist");

            var newUser = new ApplicationUser {UserName = model.Username, RefreshToken = GenerateRefreshToken()};

            var identityResult = await _userManager.CreateAsync(newUser, model.Password);
            if (!identityResult.Succeeded)
                throw new ServerErrorException($"User creation error. {string.Join(",", identityResult.Errors)}");

            var resultModel = new AuthResultModel
            {
                Token = await GenerateUserToken(newUser),
                Expire = DateTime.UtcNow.AddDays(7),
                UserId = newUser.Id,
                RefreshToken = newUser.RefreshToken
            };

            return resultModel;
        }

        public async Task<AuthResultModel> AuthorizeUser(SignInModel model)
        {
            var user = await _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == model.Username);
            if (user == null)
                throw new UserNotAuthorizedException("Username or password is incorrect");

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                throw new UserNotAuthorizedException("Username or password is incorrect");

            user.RefreshToken = GenerateRefreshToken();
            await _userManager.UpdateAsync(user);

            var userPermissions = user.UserRoles.SelectMany(_ => _.Role.Permissions.Split(","));

            var resultModel = new AuthResultModel
            {
                Token = await GenerateUserToken(user),
                Expire = DateTime.UtcNow.AddDays(7),
                UserId = user.Id,
                Permissions = new List<string>(userPermissions),
                RefreshToken = user.RefreshToken
            };

            return resultModel;
        }

        public async Task<AuthResultModel> RefreshUserToken(RefreshRequestModel refreshRequest)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["App:Auth:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(refreshRequest.Token, tokenValidationParameters, out var securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new BadRequestException("Invalid token");

            var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new BadRequestException("Invalid token");

            user.RefreshToken = GenerateRefreshToken();
            await _userManager.UpdateAsync(user);

            var resultModel = new AuthResultModel
            {
                Token = await GenerateUserToken(user),
                Expire = DateTime.UtcNow.AddDays(7),
                UserId = user.Id,
                RefreshToken = user.RefreshToken
            };

            return resultModel;
        }

        public bool IsAuthorized()
        {
            var claim = _contextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier);
            return !string.IsNullOrEmpty(claim?.Value);
        }

        public Guid GetAuthorizedUserId()
        {
            if (_contextAccessor.HttpContext.User == null)
                throw new UserNotAuthorizedException();

            var claim = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(claim?.Value))
                throw new UserNotAuthorizedException();

            return Guid.Parse(claim.Value);
        }

        public async Task<ApplicationUser> GetAuthorizedUser()
        {
            var userId = GetAuthorizedUserId();

            var user = await DbContext.Users.FindAsync(userId);
            if (user == null)
                throw new UserNotAuthorizedException();

            return user;
        }

        private async Task<string> GenerateUserToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.ASCII.GetBytes(_configuration["App:Auth:Key"]);

            var userPermissions = user?.UserRoles?.SelectMany(_ => _.Role.Permissions.Split(","));

            var claims = new List<Claim> {new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())};
            claims.AddRange(user.UserRoles?.Select(_ => new Claim(ClaimTypes.Role, _.Role.Name)) ?? Enumerable.Empty<Claim>());
            claims.AddRange(userPermissions?.Select(permission => new Claim("Permission", permission)) ?? Enumerable.Empty<Claim>());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return jwt;
        }

        private string GenerateRefreshToken()
        {
            var guid = Guid.NewGuid();
            var token = Convert.ToBase64String(guid.ToByteArray());
            return token;
        }
    }
}
