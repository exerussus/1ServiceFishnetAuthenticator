
using System;
using FishNet.Connection;

namespace Source.Features.GameAuthentication.Server.Models
{
    public class ConnectionContext
    {
        public string NickName { get; set; }
        public NetworkConnection NetworkConnection { get; set; }
        public Authenticator Authenticator { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool DataApproved { get; set; }
        public float KickTime { get; set; }
        public Action<ConnectionContext> OnClientDisconnectedAction { get; set; }
        public object Data;

        public static implicit operator NetworkConnection(ConnectionContext context) => context.NetworkConnection;
        public static implicit operator Authenticator(ConnectionContext context) => context.Authenticator;
    }

    public static class ConnectionContextExtensions
    {
        public static void SetData<T>(this ConnectionContext context, T data) => context.Data = data;
        public static T GetData<T>(this ConnectionContext context) => (T)context.Data;
        public static bool TryGetData<T>(this ConnectionContext context, out T data)
        {
            if (context.Data == null)
            {
                data = default;
                return false;
            }

            if (context.Data is not T castData)
            {
                data = default;
                return false;
            }

            data = castData;
            return true;
        }
    }
}