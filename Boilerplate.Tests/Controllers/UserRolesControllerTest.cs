using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Boilerplate.IntegrationTests.Controllers
{
    public class UserRolesControllerTest: IntegrationTestBase
    {
        public UserRolesControllerTest(Factory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CanFetchPermissionsList()
        {
            await SignUpUser("Administrator");

            var response = await Get<IEnumerable<string>>("/api/v1/user-roles/permissions");
            AssertSuccesDataResponse(response);

            response.Data.Should().HaveCountGreaterThan(0);
        }
    }
}
