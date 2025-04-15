using System;
using Exerussus._1EasyEcs.Scripts.Core;
using Exerussus.Servecies;
using FishNet.Broadcast;
using FishNet.Managing;
using FishNet.Managing.Client;
using FishNet.Transporting;
using UnityEngine;

namespace Source.Features.GameAuthentication.Client
{
    public class NetworkConnectionService : Service
    {
        [InjectSharedObject] private NetworkManager _networkManager;
        [InjectSharedObject] private ClientManager _clientManager;

        private Action _onConnectedDataSending;
        private bool _isRunning;
        
        public override void Initialize()
        {
            _clientManager.OnClientConnectionState += OnConnected;
        }

        public void RunConnection<T>(T data) where T : struct, IBroadcast
        {
            if (_isRunning)
            {
                Debug.LogError($"NetworkConnectionService.RunConnection | Connection is already running!");
                return;
            }
            
            _isRunning = true;
            _onConnectedDataSending = () => { _clientManager.Broadcast(data); };
            _clientManager.StartConnection();
        }

        private void OnConnected(ClientConnectionStateArgs data)
        {
            if ((data.ConnectionState & LocalConnectionState.Started) != 0)
            {
                _onConnectedDataSending?.Invoke();
            }
            else if ((data.ConnectionState & LocalConnectionState.Stopped) != 0)
            {
                _isRunning = false;
            }
        }
    }
}