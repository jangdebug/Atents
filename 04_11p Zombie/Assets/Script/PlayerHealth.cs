using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI를 사용하기 위하여 추가.


//public class PlayerHealth : MonoBehaviour
public class PlayerHealth : LivingEntity
{
    [SerializeField] private Slider healthSlider; // 체력 표시를 할 UI Slider
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
            healthSlider.maxValue = maxHealth; // 최대 체력.
            healthSlider.value = health; // 현재 체력
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
        healthSlider.value = health; // 데미지 적용 후, health 정보를 받아서 갱신.
    }
    // 체력 회복용 아이템을 사용 할 경우 호출.
    // value 값 만큼 체력을 회복 시킨다.

    public override void RestoreHealth(float value)
    {
        base.RestoreHealth(value);
        healthSlider.value = health; // 회복 적용 후, health 정보를 받아서 갱신.
    }

    private void OnTriggerEnter(Collider other)
    {
        // 모든 Item들은 추상 클래스 BaseItem을 상속 받기 때문에,
        // GetComponenet를 이용하여 BaseItem을 찾아 *.Use() 함수를 호출하면,
        // 지금 접촉한 Item의 Use() 함수가 실행된다.
        var item = other.GetComponent<BaseItem>();
        if (item)
        {
            item.Use(gameObject);
            if (audioSource && pickUpItemSound) audioSource.PlayOneShot(pickUpItemSound);
        }
    }

}
