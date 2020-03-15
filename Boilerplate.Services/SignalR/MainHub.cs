using System;
using System.Threading.Tasks;
using Boilerplate.Services.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace Boilerplate.Services.SignalR
{
    public class MainHub: Hub
    {
        private readonly IAuthService _authService;
        private readonly IChatService _chatService;
        private readonly IChatProvider _provider;
        private readonly IChatConnectionsStore _connectionsStore;

        public MainHub(IAuthService authService, IChatService chatService,
            IChatProvider provider, IChatConnectionsStore connectionsStore)
        {
            _authService = authService;
            _chatService = chatService;
            _provider = provider;
            _connectionsStore = connectionsStore;
        }

        public override async Task OnConnectedAsync()
        {
            //if (!_authService.IsAuthorized())
            //{
            //    Context.Abort();
            //    return;
            //}

            await Task.Run(async () =>
            {
                while (true)
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("omMessageReceived", new {message = "hello"});
                    await Task.Delay(1000);
                }
            });

            //await _connectionsStore.StoreUserConnection(Context.ConnectionId);

            //var user = await _authService.GetAuthorizedUser();
            //var userChannels = await _chatService.GetUserChannels(user.Id);
            //foreach (var channel in userChannels)
            //{
            //    await Groups.AddToGroupAsync(Context.ConnectionId, channel.Id.ToString());
            //    await _provider.UserConnectedToServer(user, channel.Id);
            //}               
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (!_authService.IsAuthorized())
                return;

            var user = await _authService.GetAuthorizedUser();
            var userChannels = await _chatService.GetUserChannels(user.Id);
            foreach (var channel in userChannels)
            {
                await _provider.UserDisconnectedFromServer(user, channel.Id);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, channel.Id.ToString());
            }

            await _connectionsStore.RemoveUserConnection();
        }
    }
}
