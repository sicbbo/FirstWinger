using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : Actor
{
    private const string playerHUDPath = "Prefabs/PlayerHUD";

    [SerializeField]
    [SyncVar]
    private Vector3 moveVector = Vector3.zero;
    [SerializeField]
    private NetworkIdentity networkIdentity = null;
    [SerializeField]
    private float speed = 0f;
    [SerializeField]
    private BoxCollider boxCollider = null;
    [SerializeField]
    private Transform fireTrans = null;
    [SerializeField]
    private float bulletSpeed = 1f;

    private InputController inputController = new InputController();

    [SerializeField]
    [SyncVar]
    private bool host = false;

    [SerializeField]
    private Material clientPlayerMaterial = null;

    protected override void Initialize()
    {
        base.Initialize();

        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        if (isLocalPlayer == true)
            inGameSceneMain.Player = this;

        if (isServer == true && isLocalPlayer == true)
        {
            host = true;
            RpcSetHost();
        }

        Transform startTrans;
        if (host == true)
            startTrans = inGameSceneMain.PlayerStartTrans1;
        else
        {
            startTrans = inGameSceneMain.PlayerStartTrans2;
            MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
            meshRenderer.material = clientPlayerMaterial;
        }

        SetPosition(startTrans.position);

        if (actorInstanceID != 0)
            inGameSceneMain.ActorManager.Regist(actorInstanceID, this);

        InitializePlayerHUD();
    }

    private void InitializePlayerHUD()
    {
        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        GameObject obj = Resources.Load<GameObject>(playerHUDPath);
        GameObject objInstance = Instantiate<GameObject>(obj, inGameSceneMain.DamageManager.CanvasTrans);
        PlayerHUD playerHUD = objInstance.GetComponent<PlayerHUD>();
        playerHUD.Initialize(this);
    }

    protected override void UpdateActor()
    {
        UpdateInput();
        UpdateMove();
    }

    [ClientCallback]
    public void UpdateInput()
    {
        inputController.UpdateInput();
    }

    private void UpdateMove()
    {
        if (moveVector.sqrMagnitude == 0)
            return;

        if (isServer == true)
            RpcMove(moveVector);
        else
        {
            CmdMove(moveVector);
            if (isLocalPlayer == true)
                transform.position += AdjustMoveVector(moveVector);
        }
    }

    [Command]
    public void CmdMove(Vector3 _moveVec)
    {
        moveVector = _moveVec;
        transform.position += moveVector;
        base.SetDirtyBit(1);
        moveVector = Vector3.zero;
    }

    [ClientRpc]
    public void RpcMove(Vector3 _moveVec)
    {
        moveVector = _moveVec;
        transform.position += moveVector;
        base.SetDirtyBit(1);
        moveVector = Vector3.zero;
    }

    private Vector3 AdjustMoveVector(Vector3 _moveVec)
    {
        Transform mainBGTrans = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().MainBGTrans;
        Vector3 result = Vector3.zero;

        result = boxCollider.transform.position + boxCollider.center + _moveVec;

        if (result.x - boxCollider.size.x * 0.5f < -mainBGTrans.localScale.x * 0.5f)
            _moveVec.x = 0f;
        if (result.x + boxCollider.size.x * 0.5f > mainBGTrans.localScale.x * 0.5f)
            _moveVec.x = 0f;
        if (result.y - boxCollider.size.y * 0.5f < -mainBGTrans.localScale.y * 0.5f)
            _moveVec.y = 0f;
        if (result.y + boxCollider.size.y * 0.5f > mainBGTrans.localScale.y * 0.5f)
            _moveVec.y = 0f;

        return _moveVec;
    }

    public void ProcessInput(Vector3 _moveDirection)
    {
        moveVector = _moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            if (enemy.IsDead == false)
            {
                BoxCollider box = (BoxCollider)other;
                Vector3 crashPos = enemy.transform.position + box.center;
                crashPos.x += box.size.x * 0.5f;

                enemy.OnCrash(crashDamage, crashPos);
            }
        }
    }

    public void FireBullet()
    {
        if (host == true)
        {
            Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletType.PlayerBullet);
            bullet.FireBullet(actorInstanceID, fireTrans.position, fireTrans.right, bulletSpeed, damage);
        }
        else
        {
            CmdFire(actorInstanceID, fireTrans.position, fireTrans.right, bulletSpeed, damage);
        }
    }

    [Command]
    public void CmdFire(int _actorInstanceID, Vector3 _firePosition, Vector3 _fireDirection, float _bulletSpeed, int _damage)
    {
        Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletType.PlayerBullet);
        bullet.FireBullet(_actorInstanceID, _firePosition, _fireDirection, _bulletSpeed, _damage);
        base.SetDirtyBit(1);
    }

    protected override void DecreaseHP(int _value, Vector3 _damagePos)
    {
        base.DecreaseHP(_value, _damagePos);

        Vector3 damagePoint = _damagePos + Random.insideUnitSphere * 0.5f;
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageManager.GenerateDamage(DamageType.Normal, damagePoint, _value, Color.red);
    }

    protected override void OnDead()
    {
        base.OnDead();
        gameObject.SetActive(false);
    }

    [ClientRpc]
    public void RpcSetHost()
    {
        host = true;
        base.SetDirtyBit(1);
    }
}