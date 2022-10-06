using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkingClient : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;
    private ConnectionStatus _status;
    
    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Failed,
        Connected,
        Loading,
        Loaded
    }
    
    public Task<StartGameResult> StartMultiplayerGame()
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;
        return _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "TestRoom3",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public Task<StartGameResult> StartSoloGame()
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;
        return _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Single,
            SessionName = "TestRoom3",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
    
    public void SetConnectionStatus(ConnectionStatus status, string message)
    {
        Debug.Log("New connection status - " + status + " - " + message);
        _status = status;
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player joined " + player.PlayerId);
    }

    public void SetScene(int scene)
    {
        if (_runner.IsSharedModeMasterClient || _runner.GameMode == GameMode.Single)
        {
            _runner.SetActiveScene(scene);   
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(GameplayInputManager.Instance.GetNetworkInput());
    }

    public int GetPlayerCount()
    {
        return _runner.ActivePlayers.Count();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        string message = "";
        switch (shutdownReason)
        {
            case ShutdownReason.IncompatibleConfiguration:
                message = "This room already exist in a different game mode!";
                break;
            case ShutdownReason.Ok:
                message = "User terminated network session!"; 
                break;
            case ShutdownReason.Error:
                message = "Unknown network error!";
                break;
            case ShutdownReason.ServerInRoom:
                message = "There is already a server/host in this room";
                break;
            case ShutdownReason.DisconnectedByPluginLogic:
                message = "The Photon server plugin terminated the network session!";
                break;
            default:
                message = shutdownReason.ToString();
                break;
        }
        SetConnectionStatus(ConnectionStatus.Disconnected, message);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        SetConnectionStatus(ConnectionStatus.Connected, "");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        SetConnectionStatus(ConnectionStatus.Disconnected, "");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        request.Accept();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        SetConnectionStatus(ConnectionStatus.Failed, reason.ToString());
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
}
