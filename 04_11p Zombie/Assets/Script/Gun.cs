using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum State
{
    Ready, // �߻� �غ��
    Empty, // źâ�� ��
    Reloading // ������ ��
}



public class Gun : MonoBehaviour
{
    private Transform pivot;


    [Header("SFX")]
    [SerializeField] private ParticleSystem muzzleFlashEffect; // �ѱ��� ȭ�� ȿ��.
    [SerializeField] private ParticleSystem shellEjectEffect; // ź�� ���� ȿ��.
    [SerializeField] private AudioClip shootSound; // �� ���� ȿ����.
    [SerializeField] private AudioClip reloadSound; // źâ ������ ȿ����.
    private AudioSource audioSource;


    public void Fire()
    {
        if (state.Equals(State.Ready) && Time.time >= lastFireTime + timeBetFire)
        // ���� ���� �� �� �ִ� �������� Ȯ��.
        {
            lastFireTime = Time.time; // �߻� ���� ����.
            Shot(); // �߻� ó��.
        }
    }


    [Header("Gun")] // Inspector�� ǥ��, �ش� Data���� ���ó�� �˷��ش�.
    [SerializeField] private Transform firePos; // �Ѿ� �߻� ��ġ.
    [SerializeField] private Transform leftHandMount; // �޼� ��ġ ����.
    [SerializeField] private Transform rightHandMount; // ������ ��ġ ����.

    [Header("IK")] //[Range(a, b)] : ���� ������ ����(a~b).
    [SerializeField] [Range(0, 1)] private float leftHandPosWeight = 1f;
    [SerializeField] [Range(0, 1)] private float leftHandRoWeight = 1f;
    [SerializeField] [Range(0, 1)] private float rightHandPosWeight = 1f;
    [SerializeField] [Range(0, 1)] private float rightHandRoWeight = 1f;


    private State state = State.Ready; // ���� ���� ���� ����.
    [Header("Gun �Ӽ�")]
    [SerializeField] private float hitRange = 50f; // �����Ÿ�.
    [SerializeField] private float timeBetFire = 0.12f; // �Ѿ� �߻� ����.
    private float lastFireTime; // ���� ���������� �߻��� ����.
    private LineRenderer bulletLineRenderer; // �Ѿ� ������ �׸��� ���� ����.
    private readonly int magCapacity = 25; // źâ �뷮.
    public int ammoRemain { get; private set; } = 100; // �����ϰ� �ִ� �Ѿ��� ��.
    public int magAmmo { get; private set; } // ���� źâ�� �����ִ� �Ѿ��� ��.
    [SerializeField] private float reloadTime = 0.9f; // ������ �ҿ� �ð�.





    public Vector3 Pivot { get { return (pivot) ? pivot.position : Vector3.zero; } set { if (pivot) pivot.position = value; } }
    public Vector3 LeftHandMountPos { get { return (leftHandMount) ? leftHandMount.position : Vector3.zero; } }
    public Quaternion LeftHandMountRo { get { return (leftHandMount) ? leftHandMount.rotation : Quaternion.identity; } }
    public Vector3 RightHandMountPos { get { return (rightHandMount) ? rightHandMount.position : Vector3.zero; } }
    public Quaternion RightHandMountRo { get { return (rightHandMount) ? rightHandMount.rotation : Quaternion.identity; } }
    public float LeftHandPosWeight { get { return leftHandPosWeight; } }
    public float LeftHandRoWeight { get { return leftHandRoWeight; } }
    public float RightHandPosWeight { get { return rightHandPosWeight; } }
    public float RightHandRoWeight { get { return rightHandRoWeight; } }


    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        pivot = transform.parent;

        bulletLineRenderer = GetComponent<LineRenderer>();
        if (bulletLineRenderer)
        {
            bulletLineRenderer.positionCount = 2; // ���� �׸��� ���� �� ���� ����.
            bulletLineRenderer.enabled = false; // ���� ��� ������ ������ ������ �ʵ��� ��Ȱ��ȭ.
        }
        magAmmo = magCapacity; // �ʱ� źâ�� �Ѿ��� �ִ�ġ��.
        state = State.Ready;
        lastFireTime = Time.time - timeBetFire;
    }


    [SerializeField] private float damage = 25; // ������ ���ݷ�.

    private void Shot()
    {
        if (firePos)
        {
            RaycastHit hit; // Physics.Raycast()�� �̿��Ͽ� �浹 ���� ������ �˾ƿ´�.
            Vector3 hitPos = firePos.position + firePos.forward * hitRange; 
            // �Ѿ��� ���� ��ġ�� ����, �ִ� �Ÿ� ��ġ�� �⺻ ������ ������.
            if (Physics.Raycast(firePos.position, firePos.forward, out hit, hitRange))
            {
                // TODO : Enemy(Zombie) Damageable Code �߰�
                IDamageable target = hit.collider.GetComponent<IDamageable>();
                if (null != target) target.OnDamage(damage, hit.point, hit.normal);

                hitPos = hit.point; // ���� �Ѿ��� ���� �������� ����.
            }
            StartCoroutine(ShotEffect(hitPos)); // �Ѿ� �߻� ����Ʈ.
            magAmmo--; // źâ�� �Ѿ��� ����.
            if (0 >= magAmmo) state = State.Empty; 
            // ���� �Ѿ� �� Ȯ��. �Ѿ��� ���ٸ�, ���� ���¸� Empty�� ����.
        }
    }

    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        if (muzzleFlashEffect) muzzleFlashEffect.Play();
        if (shellEjectEffect) shellEjectEffect.Play();
        if (audioSource && shootSound) audioSource.PlayOneShot(shootSound);


        if (bulletLineRenderer)
        {
            bulletLineRenderer.SetPosition(0, firePos.position); // �Ѿ��� �߻� ��������,
            bulletLineRenderer.SetPosition(1, hitPosition); // �Ѿ��� ���� ��ġ���� ���� �׸���.
            bulletLineRenderer.enabled = true; // �׸��� �׸������� LineRenderer�� Ȱ��ȭ ��Ų��.
        }
        yield return new WaitForSeconds(0.03f); // 0.03�� ���� ��� ó���� ���.
        if (bulletLineRenderer) bulletLineRenderer.enabled = false; 
        // LineRenderer�� ��Ȱ��ȭ�Ͽ� �Ѿ� ������ �����.
    }

    public bool Reload()
    {
        // ������ ���̰ų� ���� ź���� ���ų�,
        // ź���� �������� ���̻� �߰��� �� ���� ���� ������ �Ұ���.
        if (state.Equals(State.Reloading) || 0 >= ammoRemain || magCapacity <= magAmmo) return false;
        StartCoroutine(ReloadRoutine()); // ������ ���·� ����.
        return true;
    }
    private IEnumerator ReloadRoutine()
    {
        // ���� ���¸� ������ �� ���·� ��ȯ.
        state = State.Reloading;
        if (audioSource && reloadSound) audioSource.PlayOneShot(reloadSound);

        state = State.Reloading; // ���� ���¸� ������ �� ���·� ��ȯ.
        yield return new WaitForSeconds(reloadTime); // ������ �ҿ� �ð� ��ŭ ó���� ����.
                                                     // źâ�� ä�� �Ѿ��� ���.
                                                     // �����ϰ� �ִ� �Ѿ˰� źâ�� �� ���� ���� Ȯ���Ͽ� ���� ���� ���.
        int ammoTofill = Mathf.Min(magCapacity - magAmmo, ammoRemain);
        ammoRemain -= ammoTofill; // �����ϰ� �ִ� �Ѿ��� ���� ����.
        magAmmo += ammoTofill; // ������ �Ѿ��� ����ŭ źâ�� �Ѿ��� �߰�.
        state = State.Ready; // ���� ���� ���¸� �߻� �غ�� ���·� ����.
    }

    public void AddAmmo(int value)
    {
        ammoRemain += value;
    }
}
