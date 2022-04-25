using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//public class Enemy : MonoBehaviour
public class Enemy : LivingEntity
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] [Range(0, 100)] private float searchRange = 20;
    [SerializeField] private NavMeshAgent agent;
    private Animator anim;

    private new Collider collider;


    [SerializeField] private AudioClip deathSound; // ��� ȿ����.
    [SerializeField] private AudioClip hitSound; //�ǰ� ȿ����.
    [SerializeField] private ParticleSystem hitEffect; // �ǰ� ����Ʈ.
    private AudioSource audioSource; // ȿ������ ����ϴµ� ���.

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, searchRange);
    }
    private void Awake()
    {
        anim = GetComponent<Animator>();

        collider = GetComponent<Collider>(); // Collider�� ������ �Ű澲�� �ʴ´�.

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // �÷��� ��, ���� ������� �ʵ��� �Ѵ�.

        OnDeath += () =>
        {
            // �� �̻� �ǰ� ������ ���� �ʰ� collider�� ����.
            if (collider) collider.enabled = false;
            if (agent) agent.isStopped = true; // navigation ����.
            if (anim) anim.SetBool("isDead", isDead); // Zombie Death �ִϸ��̼� ����.

            if (audioSource && deathSound) audioSource.PlayOneShot(deathSound); // ��� ȿ���� 1ȸ ���.

            GameMgr.Instance.AddScore(100); // enemy óġ ��, 100 score ���. 
            EnemyMgr.Instance.DecreaseSpawnCount(); // enemy óġ ��, Spawn Count ����.
        };
    }

    //private void OnEnable()
    protected override void OnEnable()
    {
        base.OnEnable(); // LivingEntity�� OnEnable() ȣ��.
        if (anim) anim.SetBool("isDead", isDead); // ��� ���¸� false. isDead=false.
        if (collider) collider.enabled = true; // �ǰ��� ���� �� �ֵ��� collider�� Ȱ��ȭ.


        // ������Ʈ�� Ȱ��ȭ �� ���(Respawn), target�� ã�� �̵�.
        if (agent) agent.isStopped = false;
        StartCoroutine(UpdatePath());
    }




    private float damage = 20f; // ���ݷ�
    private IEnumerator UpdatePath()
    {
        //while (true)
        while (!isDead)
        {
            if (agent)
            {
                var targets = Physics.OverlapSphere(transform.position, searchRange, targetLayer); 
                // ������ Ž�� ���� ���� Target(Player)�� �ִ� �� Ȯ��.
                if (null != targets && 0 < targets.Length)
                {
                    var livingEntity = targets[0].GetComponent<LivingEntity>(); // �߰�.

                    if (livingEntity && !livingEntity.isDead)
                    {// ����� �����ϰ� ���� �ʾ��� ���
                        //var targetPos = targets[0].transform.position;
                        var targetPos = livingEntity.transform.position;
                        agent.SetDestination(targetPos); // �ش� Target�� ���Ͽ� �̵�.
                        

                        if (Vector3.Distance(targetPos, transform.position) <= agent.stoppingDistance)
                        //���� �Ÿ�(stoppingDistance)��ŭ �ٰ����� ���,
                        {
                            targetPos.y = transform.position.y;
                            var dir = (targetPos - transform.position).normalized;
                            transform.rotation = Quaternion.LookRotation(dir);
                            //target�� ���Ͽ� �ٶ󺸰�,
                            
                            StartCoroutine(Attack(livingEntity)); // ������ �õ��Ѵ�
                            yield break;
                        }
                    }
                }
                // Enemy�� �����̴� �ӵ�(velocity)�� ũ��(magnitude)�� �̿��Ͽ�,
                // �����̴� �ִϸ��̼� ó���� �Ѵ�.
                if (anim) anim.SetFloat("Magnitude", agent.velocity.magnitude);
            } // if(agent)
            yield return new WaitForSeconds(0.04f);
        } // While()
    } // UpdatePath(


    private IEnumerator Attack(LivingEntity target)
    {
        if (agent && target)
        {

            var trTarget = target.transform; // target�� Transform.
            while (!isDead && !target.isDead) // true���� (!isDead && !target.isDead)�� ����.
            //while (true)
            {
                // ���� ��� ����.
                if (anim) anim.SetTrigger("Attack");
                yield return new WaitForSeconds(1.1f);
                // �ǰ� ���� Ÿ�ֿ̹� target�� ��ȿ�� �Ÿ��� �ִ��� Ȯ��.
                if (Vector3.Distance(trTarget.position, transform.position) > agent.stoppingDistance) break;
                // TODO : Player Damageable Code �߰�.

                if (isDead || target.isDead) yield break;
                var hitNormal = transform.position - trTarget.position;
                target.OnDamage(damage, Vector3.zero, hitNormal);


                yield return new WaitForSeconds(1.2f);
                // ��� ���� ��, target�� ��ȿ�� �Ÿ��� �ִ��� Ȯ��.

                if (Vector3.Distance(trTarget.position, transform.position) > agent.stoppingDistance) break;
            }
        }
        // target���� �Ÿ��� �������ٸ� �ٽ� target�� �Ѿ� ����.
        if (!isDead) StartCoroutine(UpdatePath()); // if(!isDead) ���� �߰�.
    } // Attack()

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        if (anim && !isDead)
        {
            if (hitEffect)
            {
                var hitEffectTR = hitEffect.transform;
                hitEffectTR.position = hitPoint; // ����Ʈ�� �ǰ� �������� �̵�.
                                                 // �ǰ� ���� �������� ȸ��.
                hitEffectTR.rotation = Quaternion.LookRotation(hitNormal);
                hitEffect.Play(); // ����Ʈ ���.
            }
            // �ǰ� ȿ���� 1ȸ ���.
            if (audioSource && hitSound) audioSource.PlayOneShot(hitSound);

            anim.SetTrigger("Damaged"); // �������� �԰� ���� �ʾҴٸ�, �ǰ� �ִϸ��̼� ����.
        }
    }
    public void UnactiveObject() // Zombie Death ���� �� ȣ���Ͽ� ������Ʈ�� ��Ȱ��ȭ ��Ų��.
    {
        gameObject.SetActive(false);
    }



    [SerializeField] private Renderer enemyRenderer;
    public void Setup(float damage, float maxHealth, float speed, Color color, Vector3 pos)
    {
        this.damage = damage;
        this.maxHealth = maxHealth;
        if (agent) agent.speed = speed;
        if (enemyRenderer) enemyRenderer.material.color = color;
        transform.position = pos;
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (EnemyMgr.Instance) EnemyMgr.Instance.SetPooling(this);
    }

} // class Enemy


