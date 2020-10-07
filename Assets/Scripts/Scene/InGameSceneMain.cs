using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InGameSceneMain : BaseSceneMain
{
    public GameState CurrentGameState { get { return InGameNetworkTransfer.CurrentGameState; } }

    [SerializeField]
    private Player player = null;
    public Player Player { get { if (player == null) Debug.LogWarning("Main Player is not setted!"); return player; } set { player = value; } }

    [SerializeField]
    private EffectManager effectManager = null;
    public EffectManager EffectManager { get { return effectManager; } }

    [SerializeField]
    private EnemyManager enemyManager = null;
    public EnemyManager EnemyManager { get { return enemyManager; } }

    [SerializeField]
    private BulletManager bulletManager = null;
    public BulletManager BulletManager { get { return bulletManager; } }

    [SerializeField]
    private DamageManager damageManager = null;
    public DamageManager DamageManager { get { return damageManager; } }

    [SerializeField]
    private SquadronManager squadronManager = null;
    public SquadronManager SquadronManager { get { return squadronManager; } }

    [SerializeField]
    private Transform mainBGTrans = null;
    public Transform MainBGTrans { get { return mainBGTrans; } }

    [SerializeField]
    private InGameNetworkTransfer inGameNetworkTransfer = null;
    public InGameNetworkTransfer InGameNetworkTransfer { get { return inGameNetworkTransfer; } }

    [SerializeField]
    private Transform playerStartTrans1 = null;
    public Transform PlayerStartTrans1 { get { return playerStartTrans1; } }

    [SerializeField]
    private Transform playerStartTrans2 = null;
    public Transform PlayerStartTrans2 { get { return playerStartTrans2; } }

    private GamePointAccumulator gamePointAccumulator = new GamePointAccumulator();
    public GamePointAccumulator GamePointAccumulator { get { return gamePointAccumulator; } }

    private PrefabCacheSystem cacheSystem = new PrefabCacheSystem();
    public PrefabCacheSystem CacheSystem { get { return cacheSystem; } }

    private ActorManager actorManager = new ActorManager();
    public ActorManager ActorManager { get { return actorManager; } }

    public void GameStart()
    {
        InGameNetworkTransfer.RpcGameStart();
    }
}