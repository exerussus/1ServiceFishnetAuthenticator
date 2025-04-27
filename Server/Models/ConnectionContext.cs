
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
        public object Data;
    }
}