﻿using System.Collections.Generic;
using System.Linq;
using Exerussus._1EasyEcs.Scripts.Core;
using Exerussus._1Extensions.Scripts.Extensions;
using Exerussus.Servecies;
using Exerussus.Servecies.Interfaces;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Logging;
using FishNet.Managing.Server;
using FishNet.Transporting;
using Source.Features.GameAuthentication.Server.Models;
using UnityEngine;

namespace Source.Features.GameAuthentication.Server
{
    public abstract class AuthenticationService : Service, IModuleUpdate
    {
        [InjectSharedObject] private ServerManager _serverManager;
        [InjectSharedObject] private NetworkManager _networkManager;
        [InjectSharedObject] private ServiceAuthenticator _serviceAuthenticator;

        private float _nextTimeUpdate;
        
        private Dictionary<int, ConnectionContext> _inProcess;
        private readonly Dictionary<int, ConnectionContext> _authenticated = new ();
        private readonly HashSet<int> _kickList = new();
        private readonly HashSet<int> _approvedList = new();

        private const float AuthenticationTimeout = 5f;
        public float UpdateDelay => 0.5f;

        // Добавлять новые аутентификаторы сюда
        protected abstract List<Authenticator> CreateAuthenticators();

        #region Methods

        public override void SetModules(ServiceCollector serviceCollector)
        {
            _inProcess = new();
            var auths =  CreateAuthenticators();
            foreach (var a in auths) a.SetProcess(_inProcess);
            var modules = auths.Cast<ServiceModule>().ToList();
            serviceCollector.Add(modules);
        }

        public override void Initialize()
        {
            _serverManager.SetAuthenticator(_serviceAuthenticator);
            _serverManager.OnRemoteConnectionState += OnConnectionStateChanged;
            _serverManager.StartConnection();
        }

        private void OnConnectionStateChanged(NetworkConnection connection, RemoteConnectionStateArgs data)
        {
            if (data.ConnectionState == RemoteConnectionState.Started) OnClientConnected(connection);
            else OnClientDisconnected(connection);
        }

        public void OnClientDisconnected(NetworkConnection connection)
        {
            _inProcess.TryPop(connection.ClientId, out _);
            _authenticated.TryPop(connection.ClientId, out _);
        }

        public void OnClientConnected(NetworkConnection connection)
        {
            var context = new ConnectionContext
            {
                KickTime = Time.time + AuthenticationTimeout,
                NetworkConnection = connection
            };
            
            _inProcess.Add(connection.ClientId, context);
            Debug.Log($"Added client {connection.ClientId} to authentication queue.");
        }

        public void Update(float deltaTime)
        {
            foreach (var (clientId, context) in _inProcess)
            {
                if (context.DataApproved)
                {
                    _approvedList.Add(clientId);
                    continue;
                }
                
                if (context.KickTime < Time.time)
                {
                    _kickList.Add(clientId);
                }
            }
            
            if (_kickList.Count > 0)
            {
                foreach (var clientId in _kickList)
                {
                    if (!_inProcess.TryPop(clientId, out var context)) continue;
                    context.NetworkConnection.Kick(KickReason.Unset, LoggingType.Common, "Authentication timeout.");
                }

                _kickList.Clear();
            }

            if (_approvedList.Count > 0)
            {
                foreach (var clientId in _approvedList)
                {
                    if (!_inProcess.TryPop(clientId, out var context)) continue;
                    _authenticated[clientId] = context;
                    context.IsAuthenticated = true;
                    _serviceAuthenticator.SetAuthResult(context.NetworkConnection, true);
                    context.Authenticator.OnAuthenticationSuccess(context);
                }

                _approvedList.Clear();
            }
        }

        #endregion
    }
}