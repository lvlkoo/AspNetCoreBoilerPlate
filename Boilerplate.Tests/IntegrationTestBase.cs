using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Boilerplate.DAL.Entities;
using Boilerplate.Models;
using Boilerplate.Models.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Xunit;

namespace Boilerplate.Tests
{
    public class IntegrationTestBase : IClassFixture<Factory>
    {
        protected readonly Factory Factory;
        private readonly HttpClient _client;
        private string _userToken;

        public IntegrationTestBase(Factory factory)
        {
            Factory = factory;
            _client = Factory.CreateClient();
        }

        private void SetupClinet()
        {
            if (!string.IsNullOrEmpty(_userToken))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userToken);
        }

        protected async Task<BaseResponse<TResponse>> Get<TResponse>(string uri)
        {
            SetupClinet();
            var resposne = await _client.GetAsync(uri);
            return await PrepareResponse<TResponse>(resposne);
        }

        protected async Task<BaseResponse<TResponse>> Post<TResponse, TRequest>(string uri, TRequest data)
        {
            SetupClinet();
            var serialized = JsonConvert.SerializeObject(data);
            var response =
                await _client.PostAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));

            return await PrepareResponse<TResponse>(response);
        }

        protected async Task<BaseResponse<TModel>> Post<TModel>(string uri, TModel data)
        {
            SetupClinet();
            var serialized = JsonConvert.SerializeObject(data);
            var response =
                await _client.PostAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));

            return await PrepareResponse<TModel>(response);
        }

        protected async Task<BaseResponse<TResponse>> Put<TResponse, TRequest>(string uri, TRequest data)
        {
            SetupClinet();
            var serialized = JsonConvert.SerializeObject(data);
            var response =
                await _client.PutAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));

            return await PrepareResponse<TResponse>(response);
        }

        protected async Task<BaseResponse<TModel>> Put<TModel>(string uri, TModel data)
        {
            SetupClinet();
            var serialized = JsonConvert.SerializeObject(data);
            var response =
                await _client.PutAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));

            return await PrepareResponse<TModel>(response);
        }

        protected async Task<Response> Delete(string uri)
        {
            SetupClinet();
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

        private async Task<Response> PrepareResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new HttpRequestException("The specified uri was not found");

            var stringContent = await response.Content.ReadAsStringAsync();
            var modelResult = JsonConvert.DeserializeObject<Response>(stringContent);
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

            var response = await Post<AuthResultModel, SignUpModel>("/api/v1/auth/signup", model);
            AssertSuccesDataResponse(response);

            response.Data.Token.Should().NotBeNullOrEmpty();
            response.Data.RefreshToken.Should().NotBeNullOrEmpty();
            response.Data.UserId.Should().NotBeEmpty();

            _userToken = response.Data.Token;

            return response.Data;
        }

        protected async Task<AuthResultModel> SigInUser(string username, string password)
        {
            var model = new SignInModel
            {
                Username = username,
                Password = password
            };

            var response = await Post<AuthResultModel, SignInModel>("/api/v1/auth/signin", model);
            AssertSuccesDataResponse(response);

            response.Data.Token.Should().NotBeNullOrEmpty();
            response.Data.UserId.Should().NotBeEmpty();

            _userToken = response.Data.Token;

            return response.Data;
        }

        protected async Task CreateAdminUser(string userName, string password)
        {
            await CreateUser(userName, password);
        }

        protected async Task CreateWorkspaceAdminUser(string userName, string password)
        {
            await CreateUser(userName, password);
        }

        protected async Task CreateUser(string userName, string password, params string[] roles)
        {
            using (var scope = Factory.Server.Host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var adminUser = new ApplicationUser
                {
                    UserName = userName
                };

                var identityResult = await userManager.CreateAsync(adminUser, password);
                identityResult.Succeeded.Should().BeTrue();

                identityResult = await userManager.AddToRolesAsync(adminUser, roles);
                identityResult.Succeeded.Should().BeTrue();
            }
        }
    }
}
