using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField]
    protected int maxHP = 100;
    [SerializeField]
    protected int currentHP = 100;
    [SerializeField]
    protected int damage = 1;
    [SerializeField]
    protected int crashDamage = 100;
    public int CrashDamage { get { return crashDamage; } }
    [SerializeField]
    private bool isDead = false;
    public bool IsDead { get { return isDead; } }

    private void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        currentHP = maxHP;
    }

    private void Update()
    {
        UpdateActor();
    }

    protected virtual void UpdateActor()
    {

    }

    public virtual void OnBulletHited(Actor _attacker, int _damage, Vector3 _hitPos)
    {
        DecreaseHP(_attacker, _damage, _hitPos);
    }

    public virtual void OnCrash(Actor _attacker, int _damage, Vector3 _crashPos)
    {
        DecreaseHP(_attacker, _damage, _crashPos);
    }

    protected virtual void DecreaseHP(Actor _attacker, int _value, Vector3 _damagePos)
    {
        if (isDead == true)
            return;

        currentHP = Mathf.Max(currentHP - _value, 0);

        if (currentHP == 0)
            OnDead(_attacker);
    }

    protected virtual void OnDead(Actor _killer)
    {
        isDead = true;

        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EffectManager.GenerateEffect(EffectType.DeadFx, transform.position);
    }
}