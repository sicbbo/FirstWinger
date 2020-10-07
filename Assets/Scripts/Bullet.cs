using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
    private const float lifeTime = 15f;
    [SyncVar]
    private bool isNeedMove = false;
    [SyncVar]
    private bool isHited = false;
    [SyncVar]
    private float firedTime = 0f;

    [SerializeField]
    private CapsuleCollider bulletCollider = null;
    [SyncVar]
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;
    [SyncVar]
    [SerializeField]
    private float speed = 0f;
    [SyncVar]
    [SerializeField]
    private int damage = 0;
    [SyncVar]
    [SerializeField]
    private int owerInstanceID = 0;

    [SyncVar]
    private string filePath = string.Empty;
    public string FilePath { get { return filePath; } set { filePath = value; } }

    private void Start()
    {
        if (((FWNetworkManager)FWNetworkManager.singleton).isServer == false)
        {
            InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
            transform.SetParent(inGameSceneMain.BulletManager.transform);
            inGameSceneMain.CacheSystem.Add(filePath, gameObject);
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (ProcessDisappearCondition())
            return;

        UpdateMove();
    }

    private void UpdateMove()
    {
        if (isNeedMove == false)
            return;

        Vector3 moveVector = moveDirection.normalized * speed * Time.deltaTime;
        moveVector = AdjustMove(moveVector);
        transform.position += moveVector;
    }

    public void FireBullet(int _actorInstanceID, Vector3 _firePos, Vector3 _fireDir, float _speed, int _damage)
    {
        moveDirection = _fireDir;
        speed = _speed;
        damage = _damage;
        owerInstanceID = _actorInstanceID;
        SetPosition(_firePos);

        isNeedMove = true;
        firedTime = Time.time;

        UpdateNetworkBullet();
    }

    private Vector3 AdjustMove(Vector3 _moveVec)
    {
        RaycastHit hitInfo;
        if (Physics.Linecast(transform.position, transform.position + _moveVec, out hitInfo))
        {
            _moveVec = hitInfo.point - transform.position;
            OnBulletCollision(hitInfo.collider);
        }

        return _moveVec;
    }

    private void OnBulletCollision(Collider _collider)
    {
        if (isHited == true)
            return;

        if (_collider.gameObject.layer == LayerMask.NameToLayer("EnemyBullet") ||
            _collider.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            return;
        }

        Actor ower = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ActorManager.GetActor(owerInstanceID);
        Actor actor = _collider.GetComponentInParent<Actor>();
        if (actor != null && actor.IsDead == true || actor.gameObject.layer == ower.gameObject.layer)
            return;

        actor.OnBulletHited(damage, transform.position);

        isHited = true;
        isNeedMove = false;

        bulletCollider.enabled = false;

        GameObject obj = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EffectManager.GenerateEffect(EffectType.DisappearFx, transform.position);
        obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        Disappear();
    }

    private void OnTriggerEnter(Collider other)
    {
        OnBulletCollision(other);
    }

    private bool ProcessDisappearCondition()
    {
        if (transform.position.x > 15f || transform.position.x < -15f ||
            transform.position.y > 15f || transform.position.y < -15f)
        {
            Disappear();
            return true;
        }
        else
        if (Time.time - firedTime > lifeTime)
        {
            Disappear();
            return true;
        }

        return false;
    }

    private void Disappear()
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Remove(this);
    }

    [ClientRpc]
    public void RpcSetActive(bool _value)
    {
        this.gameObject.SetActive(_value);
        base.SetDirtyBit(1);
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

    public void UpdateNetworkBullet()
    {
        if (isServer == true)
        {
            RpcUpdateNetworkBullet();
        }
        else
        {
            CmdUpdateNetworkBullet();
        }
    }
    [Command]
    public void CmdUpdateNetworkBullet()
    {
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcUpdateNetworkBullet()
    {
        base.SetDirtyBit(1);
    }
}