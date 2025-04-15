
using FishNet.Connection;

namespace Source.Features.GameAuthentication.Server.Models
{
    public class ConnectionContext
    {
        public NetworkConnection NetworkConnection { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool DataApproved { get; set; }
        public float KickTime { get; set; }
        public object Data;
    }
}