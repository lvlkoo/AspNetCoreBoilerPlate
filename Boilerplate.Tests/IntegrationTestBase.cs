using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Boilerplate.EF;
using Boilerplate.Entities;
using Boilerplate.Models;
using Boilerplate.Models.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace Boilerplate.IntegrationTests
{
    public class IntegrationTestBase : IClassFixture<Factory>
    {
        protected readonly Factory Factory;
        protected AuthResultModel UserData { get; set; }
        private readonly HttpClient _client;

        public IntegrationTestBase(Factory factory)
        {
            Factory = factory;
            _client = Factory.CreateClient();
        }

        private void SetupClient()
        {
            if (!string.IsNullOrEmpty(UserData?.Token))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserData.Token);
        }

        protected async Task<BaseResponse<TResponse>> Get<TResponse>(string uri)
        {
            SetupClient();
            var resposne = await _client.GetAsync(uri);
            return await PrepareResponse<TResponse>(resposne);
        }

        protected async Task<TResponse> Post<TResponse, TRequest>(string uri, TRequest data)
        {
            SetupClient();
            var serialized = JsonConvert.SerializeObject(data);
            var response =
                await _client.PostAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new HttpRequestException("The specified uri or entity was not found");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new HttpRequestException("Unauthorized");

            var stringContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TResponse>(stringContent);
            return result;
        }

        protected async Task<BaseResponse<TResponse>> PostModel<TResponse, TRequest>(string uri, TRequest data)
        {
            SetupClient();
            var serialized = JsonConvert.SerializeObject(data);
            var response =
                await _client.PostAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));

            return await PrepareResponse<TResponse>(response);
        }

        protected async Task<BaseResponse<TModel>> PostModel<TModel>(string uri, TModel data)
        {
            SetupClient();
            var serialized = JsonConvert.SerializeObject(data);
            var response =
                await _client.PostAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));

            return await PrepareResponse<TModel>(response);
        }

        protected async Task<BaseResponse<TResponse>> Put<TResponse, TRequest>(string uri, TRequest data)
        {
            SetupClient();
            var serialized = JsonConvert.SerializeObject(data);
            var response =
                await _client.PutAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));

            return await PrepareResponse<TResponse>(response);
        }

        protected async Task<BaseResponse<TModel>> Put<TModel>(string uri, TModel data)
        {
            SetupClient();
            var serialized = JsonConvert.SerializeObject(data);
            var response =
                await _client.PutAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));

            return await PrepareResponse<TModel>(response);
        }

        protected async Task<BaseResponse> Delete(string uri)
        {
            SetupClient();
            var response = await _client.DeleteAsync(uri);
            return await PrepareResponse(response);
        }

        private async Task<BaseResponse<TResponse>> PrepareResponse<TResponse>(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new HttpRequestException("The specified uri was not found");

            var stringContent = await response.Content.ReadAsStringAsync();
            var modelResult = JsonConvert.DeserializeObject<BaseResponse<TResponse>>(stringContent);
            return modelResult;
        }

        private async Task<BaseResponse> PrepareResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new HttpRequestException("The specified uri was not found");

            var stringContent = await response.Content.ReadAsStringAsync();
            var modelResult = JsonConvert.DeserializeObject<BaseResponse>(stringContent);
            return modelResult;
        }

        protected void AssertSuccesResponse(BaseResponse response)
        {
            response.Should().NotBeNull();
            response.IsSuccess.Should().Be(true);
        }

        protected void AssertSuccesDataResponse<TResponse>(BaseResponse<TResponse> response)
        {
            AssertSuccesResponse(response);
            response.Data.Should().NotBeNull();
        }

        protected async Task<AuthResultModel> SignUpUser(string username, string password)
        {
            var model = new SignUpModel
            {
                Username = username,
                Password = password
            };

            var response = await PostModel<AuthResultModel, SignUpModel>("/api/v1/auth/signup", model);
            AssertSuccesDataResponse(response);

            response.Data.Token.Should().NotBeNullOrEmpty();
            response.Data.RefreshToken.Should().NotBeNullOrEmpty();
            response.Data.UserId.Should().NotBeEmpty();

            UserData = response.Data;

            return response.Data;
        }

        protected async Task<AuthResultModel> SignUpUser(params string[] roles)
        {
            var userName = GetRandomString();
            var password = GetRandomString();
            await CreateUser(userName, password, roles);
            return await SigInUser(userName, password);
        }

        protected async Task<AuthResultModel> SigInUser(string username, string password)
        {
            var model = new SignInModel
            {
                Username = username,
                Password = password
            };

            var response = await PostModel<AuthResultModel, SignInModel>("/api/v1/auth/signin", model);
            AssertSuccesDataResponse(response);

            response.Data.Token.Should().NotBeNullOrEmpty();
            response.Data.UserId.Should().NotBeEmpty();

            UserData = response.Data;

            return response.Data;
        }

        protected async Task<ApplicationUser> CreateUser(params string[] roles)
        {
            return await CreateUser(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), roles);
        }

        protected async Task<ApplicationUser> CreateUser(string userName, string password, params string[] roles)
        {
            using (var scope = Factory.Server.Host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var adminUser = new ApplicationUser
                {
                    UserName = userName,
                };

                var identityResult = await userManager.CreateAsync(adminUser, password);
                identityResult.Succeeded.Should().BeTrue();

                identityResult = await userManager.AddToRolesAsync(adminUser, roles);
                identityResult.Succeeded.Should().BeTrue();

                return adminUser;
            }
        }

        protected async Task<ApplicationUser> CreateUser(Action<ApplicationUser> overrides, string password, params string[] roles)
        {
            using (var scope = Factory.Server.Host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var adminUser = new ApplicationUser();

                overrides(adminUser);

                var identityResult = await userManager.CreateAsync(adminUser, password);
                identityResult.Succeeded.Should().BeTrue();

                identityResult = await userManager.AddToRolesAsync(adminUser, roles);
                identityResult.Succeeded.Should().BeTrue();

                return adminUser;
            }
        }

        protected async Task AddUserToRoles(ApplicationUser user, params string[] roles)
        {
            using (var scope = Factory.Server.Host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var identityResult = await userManager.AddToRolesAsync(user, roles);
                identityResult.Succeeded.Should().BeTrue();
            }
        }

        protected async Task<TEntity> CreateEntity<TEntity>(Action<TEntity> overrides) where TEntity : class, new()
        {
            var entity = new TEntity();

            overrides(entity);

            using (var scope = Factory.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await context.AddAsync(entity);
                await context.SaveChangesAsync();

                return entity;
            }
        }

        protected string GetRandomString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
