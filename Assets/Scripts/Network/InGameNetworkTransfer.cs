using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class InGameNetworkTransfer : NetworkBehaviour
{
    private const float gameReadyIntaval = 3f;

    [SyncVar]
    private GameState currentGameState = GameState.None;
    public GameState CurrentGameState { get { return currentGameState; } }

    [SyncVar]
    private float countingStartTime = 0f;

    private void Update()
    {
        float currentTime = Time.time;

        if (currentGameState == GameState.Ready)
        {
            if (currentTime - countingStartTime > gameReadyIntaval)
            {
                SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().SquadronManager.StartGame();
                currentGameState = GameState.Running;
            }
        }
    }

    [ClientRpc]
    public void RpcGameStart()
    {
        Debug.Log("RpcGameStart");
        countingStartTime = Time.time;
        currentGameState = GameState.Ready;

        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        inGameSceneMain.EnemyManager.Prepare();
        inGameSceneMain.BulletManager.Prepare();
    }
}