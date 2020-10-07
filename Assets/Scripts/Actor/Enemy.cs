using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    [SerializeField]
    private EnemyState currentState = EnemyState.None;
    [SerializeField]
    private Vector3 targetPosition = Vector3.zero;
    [SerializeField]
    private float currentSpeed = 0f;
    [SerializeField]
    private Transform fireTrans = null;
    [SerializeField]
    private float bulletSpeed = 1f;
    [SerializeField]
    private int fireRemainCount = 1;
    [SerializeField]
    private int gamePoint = 50;

    private string filePath = string.Empty;
    public string FilePath { get { return filePath; } set { filePath = value; } }

    private const float maxSpeed = 10.0f;
    private const float maxSpeedTime = 0.5f;

    private Vector3 currentVelocity;
    private float moveStartTime = 0f;

    private float lastActionUpdateTime = 0f;

    private Vector3 appearPoint = Vector3.zero;
    private Vector3 disappearPoint = Vector3.zero;

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

                player.OnCrash(this, crashDamage, crashPos);
            }
        }
    }

    public override void OnCrash(Actor _attacker, int _damage, Vector3 _crashPos)
    {
        base.OnCrash(_attacker, _damage, _crashPos);
    }

    public void FireBullet()
    {
        Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletType.EnemyBullet);
        bullet.FireBullet(this, fireTrans.position, -fireTrans.right, bulletSpeed, damage);
    }

    protected override void OnDead(Actor _killer)
    {
        base.OnDead(_killer);

        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().GamePointAccumulator.Accumulate(gamePoint);
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.RemoveEnemy(this);

        currentState = EnemyState.Dead;
    }

    protected override void DecreaseHP(Actor _attacker, int _value, Vector3 _damagePos)
    {
        base.DecreaseHP(_attacker, _value, _damagePos);

        Vector3 damagePoint = _damagePos + Random.insideUnitSphere * 0.5f;
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageManager.GenerateDamage(DamageType.Normal, damagePoint, _value, Color.magenta);
    }
}