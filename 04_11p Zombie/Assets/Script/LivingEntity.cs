using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDamageable
{
    ///<summary> 데미지 크기(damage), 맞은 지점(hitPoint),
    ///맞은 표면의 방향(hitNormal)</summary>
    void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
}

public class LivingEntity : MonoBehaviour, IDamageable
{
    [SerializeField] protected float maxHealth = 100; // 최대 체력.
    public float health { get; protected set; } // 현재 체력.
    public bool isDead { get { return (0 >= health); } } // 죽음 상태 확인.
    public event Action OnDeath; // 사망 시 발동할 이벤트.
    protected virtual void OnEnable()
    {
        health = maxHealth;
    }
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (isDead) return; // 이미 죽은 상태라면 더 이상 처리하지 않는다.
        health -= damage; // 데미지 만큼 체력 감소.
        if (isDead) Die(); // 데미지를 입어 체력이 0이하(사망 상태)라면 사망 이벤트 실행.
    }
    private void Die()
    {
        if (null != OnDeath) OnDeath(); // 등록된 사망 이벤트 실행.
    }
    public virtual void RestoreHealth(float value)
    {
        if (isDead) return; // 이미 죽은 상태에서는 체력을 회복할 수 없다.
        health = Mathf.Clamp(health + value, 0, maxHealth); // 체력은 최대치를 넘어 회복할 수 없다.
    }

}
