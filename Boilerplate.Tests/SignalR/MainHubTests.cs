using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Boilerplate.IntegrationTests.SignalR
{
    public class MainHubTests: IntegrationTestBase
    {
        public MainHubTests(Factory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CanConnectAndAuthorize()
        {
            await SignUpUser();

            var connection = await CreateConnection(UserData.Token);
            connection.Should().NotBeNull();
            connection.State.Should().Be(HubConnectionState.Connected);
        }

        private async Task<HubConnection> CreateConnection(string accessToken = null)
        {
            var hubConnection = new HubConnectionBuilder()
                .WithUrl($"{Factory.Server.BaseAddress}hubs/main", o =>
                {
                    o.HttpMessageHandlerFactory = _ => Factory.Server.CreateHandler();

                    if (accessToken != null)
                        o.AccessTokenProvider = () => Task.FromResult(accessToken);
                })
                .Build();

            await hubConnection.StartAsync();

            return hubConnection;
        }
    }
}
