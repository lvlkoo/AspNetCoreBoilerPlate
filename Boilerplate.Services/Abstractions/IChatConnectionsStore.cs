using System;
using System.Threading.Tasks;

namespace Boilerplate.Services.Abstractions
{
    public interface IChatConnectionsStore: IChatConnectionsStore<Guid, string> { }

    public interface IChatConnectionsStore<TUserId, TConnectionId>
    {
        Task StoreUserConnection(TConnectionId connectionId);
        Task StoreUserConnection(TUserId userId, TConnectionId connectionId);
        Task<TConnectionId> GetUserConnection(TUserId userId);
        Task RemoveUserConnection();
        Task RemoveUserConnection(TUserId userId);
    }
}
