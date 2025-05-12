
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Managing.Logging;
using FishNet.Managing.Server;
using FishNet.Transporting;
using UnityEngine;

namespace Source.Features.GameAuthentication.Server.Models
{
    public abstract class Authenticator<T> : Authenticator where T : struct, IBroadcast
    {
        public override void Initialize()
        {
            ServerManager.RegisterBroadcast<T>(OnAuthData, false);
            OnInitialize();
        }

        private void OnAuthData(NetworkConnection connection, T data, Channel channel)
        {
            if (!InProcess.TryGetValue(connection.ClientId, out var context))
            {
                connection.Kick(KickReason.Unset, LoggingType.Warning, "Authentication not found.");
                return;
            }

            context.Authenticator = this;
            context.OnClientDisconnectedAction = OnAuthenticatedClientDisconnected;
            
            OnDataCheck(context, data);

            context.KickTime = DataCheckTimeout + Time.time;
        }
        
        protected virtual void OnInitialize() { }
        protected abstract void OnDataCheck(ConnectionContext context, T data);
    }
}