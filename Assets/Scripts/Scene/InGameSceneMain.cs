using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSceneMain : BaseSceneMain
{
    private const float gameReadyIntaval = 3f;

    private GameState currentGameState = GameState.Ready;
    public GameState CurrentGameState { get { return currentGameState; } }

    private float sceneStartTime = 0f;

    [SerializeField]
    private Player player = null;
    public Player Player { get { if (player == null) Debug.LogError("Main Player is not setted!"); return player; } }
    private GamePointAccumulator gamePointAccumulator = new GamePointAccumulator();
    public GamePointAccumulator GamePointAccumulator { get { return gamePointAccumulator; } }
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
    private PrefabCacheSystem cacheSystem = new PrefabCacheSystem();
    public PrefabCacheSystem CacheSystem { get { return cacheSystem; } }

    protected override void OnStart()
    {
        sceneStartTime = Time.time;
    }

    protected override void UpdateScene()
    {
        base.UpdateScene();

        float currentTime = Time.time;
        if (currentGameState == GameState.Ready)
        {
            if (currentTime - sceneStartTime > gameReadyIntaval)
            {
                squadronManager.StartGame();
                currentGameState = GameState.Running;
            }
        }
    }
}