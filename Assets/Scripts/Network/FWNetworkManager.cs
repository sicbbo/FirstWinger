using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FWNetworkManager : NetworkManager
{
    public const int waitingPlayerCount = 2;
    private int playerCount = 0;

    public bool isServer { get; private set; }

    #region SERVER SIDE EVENT
    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log(string.Format("OnServerConnect Call : {0}, {1}", conn.address, conn.connectionId));
        base.OnServerConnect(conn);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log(string.Format("OnServerSceneChanged : {0}", sceneName));
        base.OnServerSceneChanged(sceneName);
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        Debug.Log(string.Format("OnServerReady : {0}, {1}", conn.address, conn.connectionId));
        base.OnServerReady(conn);

        playerCount++;

        if (playerCount >= waitingPlayerCount)
        {
            InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
            inGameSceneMain.GameStart();
        }
    }

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        Debug.Log(string.Format("OnServerError : errorCode = {0}", errorCode));
        base.OnServerError(conn, errorCode);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log(string.Format("OnServerDisconnect : {0}", conn.address));
        base.OnServerDisconnect(conn);
    }

    public override void OnStartServer()
    {
        Debug.Log("OnStartServer");
        base.OnStartServer();
        isServer = true;
    }
    #endregion

    #region CLIENT SIDE EVENT
    public override void OnStartClient(NetworkClient client)
    {
        Debug.Log(string.Format("OnStartClient : {0}", client.serverIp));
        base.OnStartClient(client);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log(string.Format("OnClientConnect : connetionId = {0}, hostId = {1}", conn.connectionId, conn.hostId));
        base.OnClientConnect(conn);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        Debug.Log(string.Format("OnClientSceneChanged : {0}", conn.hostId));
        base.OnClientSceneChanged(conn);
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log(string.Format("OnClientError : errorCode = {0}", errorCode));
        base.OnClientError(conn, errorCode);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log(string.Format("OnClientDisconnect : {0}", conn.hostId));
        base.OnClientDisconnect(conn);
    }

    public override void OnClientNotReady(NetworkConnection conn)
    {
        Debug.Log(string.Format("OnClientNotReady : {0}", conn.hostId));
        base.OnClientNotReady(conn);
    }

    public override void OnDropConnection(bool success, string extendedInfo)
    {
        Debug.Log(string.Format("OnDropConnection : {0}", extendedInfo));
        base.OnDropConnection(success, extendedInfo);
    }
    #endregion
}