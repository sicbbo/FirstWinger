using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Actor : NetworkBehaviour
{
    [SerializeField]
    [SyncVar]
    protected int maxHP = 100;
    public int MaxHP { get { return maxHP; } }
    [SerializeField]
    [SyncVar]
    protected int currentHP = 100;
    public int CurrentHP { get { return currentHP; } }
    [SerializeField]
    [SyncVar]
    protected int damage = 1;
    [SerializeField]
    [SyncVar]
    protected int crashDamage = 100;
    [SerializeField]
    [SyncVar]
    protected bool isDead = false;
    public bool IsDead { get { return isDead; } }

    [SyncVar]
    protected int actorInstanceID = 0;
    public int ActorInstanceID { get { return actorInstanceID; } }

    private void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        currentHP = maxHP;

        if (isServer == true)
        {
            actorInstanceID = GetInstanceID();
            RpcSetActorInstanceID(actorInstanceID);
        }
    }

    private void Update()
    {
        UpdateActor();
    }

    protected virtual void UpdateActor()
    {

    }

    public virtual void OnBulletHited(int _damage, Vector3 _hitPos)
    {
        DecreaseHP(_damage, _hitPos);
    }

    public virtual void OnCrash(int _damage, Vector3 _crashPos)
    {
        DecreaseHP(_damage, _crashPos);
    }

    protected virtual void DecreaseHP(int _value, Vector3 _damagePos)
    {
        if (isDead == true)
            return;

        if (isServer == true)
        {
            RpcDecreaseHP(_value, _damagePos);
        }
        else
        {
            CmdDecreaseHP(_value, _damagePos);
            if (isLocalPlayer == true)
                InternalDecreaseHP(_value, _damagePos);
        }
    }

    protected virtual void InternalDecreaseHP(int _value, Vector3 _damagePos)
    {
        if (isDead == true)
            return;

        currentHP -= _value;

        if (currentHP < 0)
            currentHP = 0;

        if (currentHP == 0)
            OnDead();
    }

    protected virtual void OnDead()
    {
        isDead = true;

        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EffectManager.GenerateEffect(EffectType.DeadFx, transform.position);
    }

    public void SetPosition(Vector3 _position)
    {
        if (isServer == true)
        {
            RpcSetPosition(_position);
        }
        else
        {
            CmdSetPosition(_position);
            if (isLocalPlayer)
                transform.position = _position;
        }
    }

    [Command]
    public void CmdSetPosition(Vector3 _position)
    {
        this.transform.position = _position;
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcSetPosition(Vector3 _position)
    {
        this.transform.position = _position;
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcSetActive(bool _value)
    {
        this.gameObject.SetActive(_value);
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcSetActorInstanceID(int _instID)
    {
        this.actorInstanceID = _instID;

        if (this.actorInstanceID != 0)
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ActorManager.Regist(this.actorInstanceID, this);

        base.SetDirtyBit(1);
    }

    [Command]
    public void CmdDecreaseHP(int _value, Vector3 _damagePos)
    {
        InternalDecreaseHP(_value, _damagePos);
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcDecreaseHP(int _value, Vector3 _damagePos)
    {
        InternalDecreaseHP(_value, _damagePos);
        base.SetDirtyBit(1);
    }
}