using System.Collections.Generic;
using Exerussus.Servecies;
using FishNet.Managing.Server;

namespace Source.Features.GameAuthentication.Server.Models
{
    public abstract class Authenticator : ServiceModule
    {
        private ServerManager _serverManager;
        protected abstract float DataCheckTimeout { get; }
        protected Dictionary<int, ConnectionContext> InProcess;

        public ServerManager ServerManager => _serverManager;

        public Authenticator SetProcess(Dictionary<int, ConnectionContext> inProcess, ServerManager serverManager)
        {
            _serverManager = serverManager;
            InProcess = inProcess;
            return this;
        }
        
        public virtual void OnAuthenticationSuccess(ConnectionContext context) { }
        protected virtual void OnAuthenticatedClientDisconnected(ConnectionContext context) { }
    }
}