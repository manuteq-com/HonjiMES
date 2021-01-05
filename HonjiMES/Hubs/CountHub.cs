using HonjiMES.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace HonjiMES.Hubs
{
    public class ChartHub : Hub
    {
        public async Task NewMessage(HubMessage msg)
        {
            await Clients.All.SendAsync("MessageReceived", msg);
        }
    }
}
