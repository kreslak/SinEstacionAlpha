using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    Dictionary<PlayerRef, NetworkObject> _players = new Dictionary<PlayerRef, NetworkObject>();
    [SerializeField] private NetworkObject _playerPrefab;
    [SerializeField] Transform[] spawnPoints;

    [SerializeField] NetworkObject cameraHold;

    NetworkRunner _runner;
    void Start()
    {
        Camera.main.GetComponent<CameraRot>().enabled = false;
    }
    async void StartGame(GameMode gameMode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await _runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            SessionName = "tp parcial 1",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    #region not used

    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
    {
    }

    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
    {
    }

    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
    {
    }

    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

#endregion

    #region OnPlayer Events
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsPlayer)
        {
            var spawnedPlayer = runner
                .Spawn(_playerPrefab, spawnPoints[_players.Count].position, Quaternion.identity, player);
            Camera.main.GetComponent<CameraRot>().orient = spawnedPlayer.GetComponent<PlayerMovement>().orient;
            Camera.main.GetComponent<CameraRot>().enabled = true;

            _players.Add(player, spawnedPlayer);
            Camera.main.GetComponent<CameraRot>().FakeStart();
            cameraHold.GetComponent<camPos>().cameraPosition = spawnedPlayer.GetComponentInChildren<xdxd>().transform;
            
        }
    }


    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_players.ContainsKey(player))
        {
            runner.Despawn(_players[player]);
            _players.Remove(player);
        }
    }
    #endregion

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        Debug.Log("toque algo");
        var inputData = new NetworkInputData();
        inputData.moveInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        inputData.shootInput = Input.GetMouseButtonDown(0);
        inputData.isJumping = Input.GetKey(KeyCode.Space);
        inputData.isSprinting = Input.GetKey(KeyCode.LeftShift);

        input.Set(inputData);
    }

    #region Buttons
    public void HostGame()
    {
        StartGame(GameMode.Host);
    }

    public void JoinGame()
    {
        StartGame(GameMode.Client);
    }
    #endregion
}

