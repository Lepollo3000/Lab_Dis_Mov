using Microsoft.AspNetCore.SignalR;
using PRUEBA.Shared;

namespace PRUEBA.Server.Hubs;

public class NotificationHub : Hub
{
    public Task RoomsUpdated(string room) => Clients.All.SendAsync(HubEndpoints.RoomsUpdated, room);
}
