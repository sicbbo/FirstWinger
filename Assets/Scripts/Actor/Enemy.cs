using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Enemy : Actor
{
    [SerializeField]
    [SyncVar]
    private EnemyState currentState = EnemyState.None;
    [SerializeField]
    [SyncVar]
    private Vector3 targetPosition = Vector3.zero;
    [SerializeField]
    [SyncVar]
    private float currentSpeed = 0f;
    [SerializeField]
    private Transform fireTrans = null;
    [SerializeField]
    [SyncVar]
    private float bulletSpeed = 1f;
    [SerializeField]
    [SyncVar]
    private int fireRemainCount = 1;
    [SerializeField]
    [SyncVar]
    private int gamePoint = 50;

    [SyncVar]
    [SerializeField]
    private string filePath = string.Empty;
    public string FilePath { get { return filePath; } set { filePath = value; } }

    private const float maxSpeed = 10.0f;
    private const float maxSpeedTime = 0.5f;

    [SyncVar]
    private Vector3 currentVelocity;
    [SyncVar]
    private float moveStartTime = 0f;

    [SyncVar]
    private float lastActionUpdateTime = 0f;

    [SyncVar]
    private Vector3 appearPoint = Vector3.zero;
    [SyncVar]
    private Vector3 disappearPoint = Vector3.zero;

    protected override void Initialize()
    {
        base.Initialize();

        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        if (((FWNetworkManager)FWNetworkManager.singleton).isServer == false)
        {
            transform.SetParent(inGameSceneMain.EnemyManager.transform);
            inGameSceneMain.CacheSystem.Add(filePath, gameObject);
            gameObject.SetActive(false);
        }

        if (actorInstanceID != 0)
            inGameSceneMain.ActorManager.Regist(actorInstanceID, this);
    }

    protected override void UpdateActor()
    {
        switch (currentState)
        {
            case EnemyState.None:
                break;
            case EnemyState.Ready:
                UpdateReady();
                break;
            case EnemyState.Battle:
                UpdateBattle();
                break;
            case EnemyState.Dead:
                break;
            case EnemyState.Appear:
            case EnemyState.Disappear:
                UpdateSpeed();
                UpdateMove();
                break;
        }
    }

    private void UpdateSpeed()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, (Time.time - moveStartTime)/maxSpeed);
    }

    private void UpdateMove()
    {
        float distance = Vector3.Distance(targetPosition, transform.position);
        if (distance == 0)
        {
            Arrived();
            return;
        }

        currentVelocity = (targetPosition - transform.position).normalized * currentSpeed;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, distance/currentSpeed, maxSpeed);
    }

    private void Arrived()
    {
        currentSpeed = 0f;
        if (currentState == EnemyState.Appear)
        {
            currentState = EnemyState.Battle;
            lastActionUpdateTime = Time.time;
        }
        else
        //if (currentState == State.Disappear)
        {
            currentState = EnemyState.None;
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.RemoveEnemy(this);
        }
    }

    public void Reset(SquadronMemberStruct _data)
    {
        if (isServer == true)
        {
            RpcReset(_data);
        }
        else
        {
            CmdReset(_data);
            if (isLocalPlayer == true)
                ResetData(_data);
        }
    }

    private void ResetData(SquadronMemberStruct _data)
    {
        EnemyStruct enemyStruct = SystemManager.Instance.EnemyTable.GetEnemyData(_data.enemyID);

        currentHP = maxHP = enemyStruct.maxHP;
        damage = enemyStruct.damage;
        crashDamage = enemyStruct.crashDamage;
        bulletSpeed = enemyStruct.bulletSpeed;
        fireRemainCount = enemyStruct.fireRemainCount;
        gamePoint = enemyStruct.gamePoint;
        appearPoint = new Vector3(_data.appearPointX, _data.appearPointY, 0f);
        disappearPoint = new Vector3(_data.disappearPointX, _data.disappearPointY, 0f);

        currentState = EnemyState.Ready;
        lastActionUpdateTime = Time.time;

        isDead = false;
    }

    private void Appear(Vector3 _targetPos)
    {
        targetPosition = _targetPos;
        currentSpeed = maxSpeed;
        currentState = EnemyState.Appear;
        moveStartTime = Time.time;
    }

    private void Disappear(Vector3 _targetPos)
    {
        targetPosition = _targetPos;
        currentSpeed = 0f;
        currentState = EnemyState.Disappear;
        moveStartTime = Time.time;
    }

    private void UpdateReady()
    {
        if (Time.time - lastActionUpdateTime > 1f)
        {
            Appear(appearPoint);
        }
    }

    private void UpdateBattle()
    {
        if (Time.time - lastActionUpdateTime > 1.0f)
        {
            if (fireRemainCount > 0)
            {
                FireBullet();
                fireRemainCount--;
            }
            else
            {
                Disappear(disappearPoint);
            }

            lastActionUpdateTime = Time.time;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            if (player.IsDead == false)
            {
                BoxCollider box = (BoxCollider)other;
                Vector3 crashPos = player.transform.position + box.center;
                crashPos.x += box.size.x * 0.5f;

                player.OnCrash(crashDamage, crashPos);
            }
        }
    }

    public void FireBullet()
    {
        Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletType.EnemyBullet);
        bullet.FireBullet(actorInstanceID, fireTrans.position, -fireTrans.right, bulletSpeed, damage);
    }

    protected override void OnDead()
    {
        base.OnDead();

        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().GamePointAccumulator.Accumulate(gamePoint);
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.RemoveEnemy(this);

        currentState = EnemyState.Dead;
    }

    protected override void DecreaseHP(int _value, Vector3 _damagePos)
    {
        base.DecreaseHP(_value, _damagePos);

        Vector3 damagePoint = _damagePos + Random.insideUnitSphere * 0.5f;
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageManager.GenerateDamage(DamageType.Normal, damagePoint, _value, Color.magenta);
    }

    [Command]
    public void CmdReset(SquadronMemberStruct _data)
    {
        ResetData(_data);
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcReset(SquadronMemberStruct _data)
    {
        ResetData(_data);
        base.SetDirtyBit(1);
    }
}