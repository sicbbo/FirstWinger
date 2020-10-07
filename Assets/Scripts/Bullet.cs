using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private const float lifeTime = 15f;
    private Actor ower = null;
    private bool isNeedMove = false;
    private bool isHited = false;
    private float firedTime = 0f;
    private int damage = 0;

    [SerializeField]
    private CapsuleCollider bulletCollider = null;
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;
    [SerializeField]
    private float speed = 0f;

    private string filePath = string.Empty;
    public string FilePath { get { return filePath; } set { filePath = value; } }

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

    public void FireBullet(Actor _ower, Vector3 _firePos, Vector3 _fireDir, float _speed, int _damage)
    {
        ower                = _ower;
        transform.position  = _firePos;
        moveDirection       = _fireDir;
        speed               = _speed;
        damage              = _damage;

        isNeedMove = true;
        firedTime = Time.time;
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

        Actor actor = _collider.GetComponentInParent<Actor>();
        if (actor != null && actor.IsDead == true || actor.gameObject.layer == ower.gameObject.layer)
            return;

        actor.OnBulletHited(ower, damage, transform.position);

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
}