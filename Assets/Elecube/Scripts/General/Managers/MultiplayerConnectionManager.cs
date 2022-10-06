using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerConnectionManager : Singleton<MultiplayerConnectionManager>
{
    private NetworkingClient _networkingClient;

    protected override void OnAwake()
    {
        GameObject clientGO = new GameObject("NetworkingClient");
        DontDestroyOnLoad(clientGO);
        _networkingClient = clientGO.AddComponent<NetworkingClient>();
    }
    
    public Task<StartGameResult> StartGame()
    {
        return _networkingClient.StartMultiplayerGame();
    }

    public void SetScene(int scene)
    {
        _networkingClient.SetScene(scene);
    }

    public int GetPlayerCount()
    {
        return _networkingClient.GetPlayerCount();
    }

    public Task<StartGameResult> StartSolo()
    {
        return _networkingClient.StartSoloGame();
    }
}
