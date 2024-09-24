using Fusion;
using Fusion.Sockets;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private NetworkRunner networkRunner;

    private void Start()
    {
        networkRunner = GetComponent<NetworkRunner>();
        StartGame();
    }

    async void StartGame()
    {
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Shared, NetAddress.Any(), SceneRef.None, null);
        await clientTask;
    }

    // Inicialización de NetworkRunner
    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        var sceneManager = runner.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null) sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        return runner.StartGame(new StartGameArgs()
        {
            GameMode = gameMode, // Modo "Shared"
            SessionName = "MultiplayerRoom",
            Scene = scene,
            SceneManager = sceneManager
        });
    }
}

