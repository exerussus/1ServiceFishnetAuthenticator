using System;
using FishNet.Connection;

namespace Source.Features.GameAuthentication.Server.Models
{
    public class ServiceAuthenticator : FishNet.Authenticating.Authenticator
    {
        public override event Action<NetworkConnection, bool> OnAuthenticationResult;
        
        public void SetAuthResult(NetworkConnection connection, bool result) => OnAuthenticationResult?.Invoke(connection, result);
    }
}