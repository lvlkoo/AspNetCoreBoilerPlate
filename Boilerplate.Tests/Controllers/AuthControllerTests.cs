using System.Threading.Tasks;
using Boilerplate.Models.Auth;
using FluentAssertions;
using Xunit;

namespace Boilerplate.IntegrationTests.Controllers
{
    public class AuthControllerTests: IntegrationTestBase
    {
        public AuthControllerTests(Factory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CanSignUp()
        {
            await SignUpUser("user1", "password1");
        }

        [Fact]
        public async Task CanSignIn()
        {
            var username = "user2";
            var password = "password2";

            await SignUpUser(username, password);
            await SigInUser(username, password);
        }

        [Fact]
        public async Task TokenRefreshes()
        {
            var username = "user3";
            var password = "password3";

            var authResult = await SignUpUser(username, password);
            
            var refreshModel = new RefreshRequestModel
            {
                Token = authResult.Token,
                RefreshToken = authResult.RefreshToken
            };
            
            var refreshResponse = await PostModel<AuthResultModel, RefreshRequestModel>("/api/v1/auth/refresh", refreshModel);
            AssertSuccesDataResponse(refreshResponse);
            
            refreshResponse.Data.Token.Should().NotBeNullOrEmpty();
            refreshResponse.Data.RefreshToken.Should().NotBeNullOrEmpty();
            refreshResponse.Data.UserId.Should().NotBeEmpty();
        }
    }
}
