using System.Collections.Generic;
using Exerussus.Servecies;

namespace Source.Features.GameAuthentication.Server.Models
{
    public abstract class Authenticator : ServiceModule
    {
        protected abstract float DataCheckTimeout { get; }
        protected Dictionary<int, ConnectionContext> _inProcess;

        public Authenticator SetProcess(Dictionary<int, ConnectionContext> inProcess)
        {
            _inProcess = inProcess;
            return this;
        }
    }
}