using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boilerplate.Services.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Boilerplate.Services.Implementations
{
    public class InMemoryChatConnectionStore: IChatConnectionsStore
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IAuthService _authService;

        public InMemoryChatConnectionStore(IMemoryCache memoryCache, IAuthService authService)
        {
            _memoryCache = memoryCache;
            _authService = authService;
        }

        public Task StoreUserConnection(string connectionId)
        {
            var userId = _authService.GetAuthorizedUserId();
            return StoreUserConnection(userId, connectionId);
        }

        public Task StoreUserConnection(Guid userId, string connectionId)
        {          
            var userConnections = _memoryCache.GetOrCreate("connections", e => new Dictionary<Guid, string>());

            if (userConnections.ContainsKey(userId))
                userConnections[userId] = connectionId;
            else
                userConnections.Add(userId, connectionId);

            return Task.CompletedTask;
        }

        public Task<string> GetUserConnection(Guid userId)
        {
            var userConnections = _memoryCache.Get<Dictionary<Guid, string>>("connections");
            if (userConnections == null)
                return Task.FromResult<string>(null);

            if (userConnections.ContainsKey(userId))
                return Task.FromResult(userConnections[userId]);

            return Task.FromResult<string>(null);
        }

        public Task RemoveUserConnection()
        {
            var userId = _authService.GetAuthorizedUserId();
            return RemoveUserConnection(userId);
        }

        public Task RemoveUserConnection(Guid userId)
        {
            var userConnections = _memoryCache.Get<Dictionary<Guid, string>>("connections");
            if (userConnections == null)
                return Task.CompletedTask;

            if (!userConnections.ContainsKey(userId))
                return Task.CompletedTask;

            userConnections.Remove(userId);
            return Task.CompletedTask;
        }
    }
}
