using System.Collections.Generic;
using Exerussus.Servecies;

namespace Source.Features.GameAuthentication.Server.Models
{
    public abstract class Authenticator : ServiceModule
    {
        protected abstract float DataCheckTimeout { get; }
        protected Dictionary<int, ConnectionContext> InProcess;

        public Authenticator SetProcess(Dictionary<int, ConnectionContext> inProcess)
        {
            InProcess = inProcess;
            return this;
        }
        
        public abstract void OnAuthenticationSuccess(ConnectionContext context);
    }
}