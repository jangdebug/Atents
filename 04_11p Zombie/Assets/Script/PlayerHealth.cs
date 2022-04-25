using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI�� ����ϱ� ���Ͽ� �߰�.


//public class PlayerHealth : MonoBehaviour
public class PlayerHealth : LivingEntity
{
    [SerializeField] private Slider healthSlider; // ü�� ǥ�ø� �� UI Slider
    [SerializeField] private AudioClip pickUpItemSound;

    private PlayerInput playerInput;
    private Animator anim;

    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        OnDeath += () =>
        {
            UIMgr.Instance.GameOver();
            if (playerInput) playerInput.enabled = false;
            if (anim) anim.SetTrigger("Die");

            if (audioSource && deathSound) audioSource.PlayOneShot(deathSound);
        };
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (healthSlider)
        {
            healthSlider.maxValue = maxHealth; // �ִ� ü��.
            healthSlider.value = health; // ���� ü��
        }
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!isDead && hitEffect)
        {
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();
            if (audioSource && hitSound) audioSource.PlayOneShot(hitSound);
        }


        base.OnDamage(damage, hitPoint, hitNormal);
        healthSlider.value = health; // ������ ���� ��, health ������ �޾Ƽ� ����.
    }
    // ü�� ȸ���� �������� ��� �� ��� ȣ��.
    // value �� ��ŭ ü���� ȸ�� ��Ų��.

    public override void RestoreHealth(float value)
    {
        base.RestoreHealth(value);
        healthSlider.value = health; // ȸ�� ���� ��, health ������ �޾Ƽ� ����.
    }

    private void OnTriggerEnter(Collider other)
    {
        // ��� Item���� �߻� Ŭ���� BaseItem�� ��� �ޱ� ������,
        // GetComponenet�� �̿��Ͽ� BaseItem�� ã�� *.Use() �Լ��� ȣ���ϸ�,
        // ���� ������ Item�� Use() �Լ��� ����ȴ�.
        var item = other.GetComponent<BaseItem>();
        if (item)
        {
            item.Use(gameObject);
            if (audioSource && pickUpItemSound) audioSource.PlayOneShot(pickUpItemSound);
        }
    }

}
