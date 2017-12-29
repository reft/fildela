using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;

namespace Fildela.Web.SignalR
{
    [HubName("ActiveUsers")]
    public class ActiveUsersHub : Hub
    {
        static int _usersConnected = 0;
        public static List<string> Users = new List<string>();

        public void RegisterUserConnect()
        {
            string clientId = GetClientId();

            if (Users.IndexOf(clientId) == -1)
            {
                Users.Add(clientId);

                _usersConnected += 1;

                this.Clients.All.onHitRecorded(_usersConnected.ToString("N0"));
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string clientId = GetClientId();

            if (Users.IndexOf(clientId) > -1)
            {
                Users.Remove(clientId);

                _usersConnected -= 1;
            }

            this.Clients.All.onHitRecorded(_usersConnected);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string clientId = GetClientId();

            if (Users.IndexOf(clientId) == -1)
            {
                Users.Add(clientId);

                _usersConnected += 1;

                this.Clients.All.onHitRecorded(_usersConnected.ToString("N0"));
            }

            return base.OnReconnected(); ;
        }

        private string GetClientId()
        {
            string clientId = "";
            if (Context.QueryString["clientId"] != null)
            {
                clientId = this.Context.QueryString["clientId"];
            }

            if (string.IsNullOrEmpty(clientId.Trim()))
            {
                clientId = Context.ConnectionId;
            }

            return clientId;
        }
    }
}