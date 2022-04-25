using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDamageable
{
    ///<summary> ������ ũ��(damage), ���� ����(hitPoint),
    ///���� ǥ���� ����(hitNormal)</summary>
    void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
}

public class LivingEntity : MonoBehaviour, IDamageable
{
    [SerializeField] protected float maxHealth = 100; // �ִ� ü��.
    public float health { get; protected set; } // ���� ü��.
    public bool isDead { get { return (0 >= health); } } // ���� ���� Ȯ��.
    public event Action OnDeath; // ��� �� �ߵ��� �̺�Ʈ.
    protected virtual void OnEnable()
    {
        health = maxHealth;
    }
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (isDead) return; // �̹� ���� ���¶�� �� �̻� ó������ �ʴ´�.
        health -= damage; // ������ ��ŭ ü�� ����.
        if (isDead) Die(); // �������� �Ծ� ü���� 0����(��� ����)��� ��� �̺�Ʈ ����.
    }
    private void Die()
    {
        if (null != OnDeath) OnDeath(); // ��ϵ� ��� �̺�Ʈ ����.
    }
    public virtual void RestoreHealth(float value)
    {
        if (isDead) return; // �̹� ���� ���¿����� ü���� ȸ���� �� ����.
        health = Mathf.Clamp(health + value, 0, maxHealth); // ü���� �ִ�ġ�� �Ѿ� ȸ���� �� ����.
    }

}
