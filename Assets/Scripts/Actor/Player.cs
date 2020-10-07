using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    [SerializeField]
    private Vector3 moveVector = Vector3.zero;
    [SerializeField]
    private float speed = 0f;
    [SerializeField]
    private BoxCollider boxCollider = null;
    [SerializeField]
    private Transform mainBGTrans = null;
    [SerializeField]
    private Transform fireTrans = null;
    [SerializeField]
    private float bulletSpeed = 1f;

    private PlayerStatePanel playerStatePanel = null;

    protected override void Initialize()
    {
        base.Initialize();
        playerStatePanel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
        playerStatePanel.SetHP(currentHP, maxHP);
    }

    protected override void UpdateActor()
    {
        UpdateMove();
    }

    private void UpdateMove()
    {
        if (moveVector.sqrMagnitude == 0)
            return;

        moveVector = AdjustMoveVector(moveVector);

        transform.position += moveVector;
    }

    private Vector3 AdjustMoveVector(Vector3 _moveVec)
    {
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

                enemy.OnCrash(this, crashDamage, crashPos);
            }
        }
    }

    public override void OnCrash(Actor _attacker, int _damage, Vector3 _crashPos)
    {
        base.OnCrash(_attacker, _damage, _crashPos);
    }

    public void FireBullet()
    {
        Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletType.PlayerBullet);
        bullet.FireBullet(this, fireTrans.position, fireTrans.right, bulletSpeed, damage);
    }

    protected override void DecreaseHP(Actor _attacker, int _value, Vector3 _damagePos)
    {
        base.DecreaseHP(_attacker, _value, _damagePos);
        playerStatePanel.SetHP(currentHP, maxHP);

        Vector3 damagePoint = _damagePos + Random.insideUnitSphere * 0.5f;
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageManager.GenerateDamage(DamageType.Normal, damagePoint, _value, Color.red);
    }

    protected override void OnDead(Actor _killer)
    {
        base.OnDead(_killer);
        gameObject.SetActive(false);
    }
}