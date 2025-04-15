using System;
using Exerussus._1EasyEcs.Scripts.Core;
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
        [InjectSharedObject] private ServerManager _serverManager;
        public readonly Type AuthDataType = typeof(T);

        public override void Initialize()
        {
            _serverManager.RegisterBroadcast<T>(OnAuthData, false);
            OnInitialize();
        }

        protected void OnAuthData(NetworkConnection connection, T data, Channel channel)
        {
            if (!_inProcess.TryGetValue(connection.ClientId, out var context))
            {
                connection.Kick(KickReason.Unset, LoggingType.Warning, "Authentication not found.");
                return;
            }
            
            try
            {
                OnAuthDataSend(context, data);
            }
            catch (Exception e)
            {
                context.NetworkConnection.Kick(KickReason.MalformedData, LoggingType.Warning, "Malformed authentication data.");
                return;
            }
            
            OnDataCheck(context, data);

            context.KickTime = DataCheckTimeout + Time.time;
        }
        
        protected abstract void OnAuthDataSend(ConnectionContext context, T data);
        protected abstract void OnDataCheck(ConnectionContext context, T data);
        protected virtual void OnInitialize() { }
    }
}