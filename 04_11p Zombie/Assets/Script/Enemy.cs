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


    [SerializeField] private AudioClip deathSound; // 사망 효과음.
    [SerializeField] private AudioClip hitSound; //피격 효과음.
    [SerializeField] private ParticleSystem hitEffect; // 피격 이펙트.
    private AudioSource audioSource; // 효과음을 출력하는데 사용.

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, searchRange);
    }
    private void Awake()
    {
        anim = GetComponent<Animator>();

        collider = GetComponent<Collider>(); // Collider의 종류를 신경쓰지 않는다.

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // 플레이 시, 사운드 실행되지 않도록 한다.

        OnDeath += () =>
        {
            // 더 이상 피격 판정이 되지 않게 collider를 끈다.
            if (collider) collider.enabled = false;
            if (agent) agent.isStopped = true; // navigation 정지.
            if (anim) anim.SetBool("isDead", isDead); // Zombie Death 애니메이션 실행.

            if (audioSource && deathSound) audioSource.PlayOneShot(deathSound); // 사망 효과음 1회 재생.

            GameMgr.Instance.AddScore(100); // enemy 처치 시, 100 score 상승. 
            EnemyMgr.Instance.DecreaseSpawnCount(); // enemy 처치 시, Spawn Count 감소.
        };
    }

    //private void OnEnable()
    protected override void OnEnable()
    {
        base.OnEnable(); // LivingEntity의 OnEnable() 호출.
        if (anim) anim.SetBool("isDead", isDead); // 사망 상태를 false. isDead=false.
        if (collider) collider.enabled = true; // 피격을 받을 수 있도록 collider를 활성화.


        // 오브젝트가 활성화 될 경우(Respawn), target을 찾아 이동.
        if (agent) agent.isStopped = false;
        StartCoroutine(UpdatePath());
    }




    private float damage = 20f; // 공격력
    private IEnumerator UpdatePath()
    {
        //while (true)
        while (!isDead)
        {
            if (agent)
            {
                var targets = Physics.OverlapSphere(transform.position, searchRange, targetLayer); 
                // 설정한 탐색 범위 내에 Target(Player)이 있는 지 확인.
                if (null != targets && 0 < targets.Length)
                {
                    var livingEntity = targets[0].GetComponent<LivingEntity>(); // 추가.

                    if (livingEntity && !livingEntity.isDead)
                    {// 대상이 존재하고 죽지 않았을 경우
                        //var targetPos = targets[0].transform.position;
                        var targetPos = livingEntity.transform.position;
                        agent.SetDestination(targetPos); // 해당 Target을 향하여 이동.
                        

                        if (Vector3.Distance(targetPos, transform.position) <= agent.stoppingDistance)
                        //일정 거리(stoppingDistance)만큼 다가갔을 경우,
                        {
                            targetPos.y = transform.position.y;
                            var dir = (targetPos - transform.position).normalized;
                            transform.rotation = Quaternion.LookRotation(dir);
                            //target을 향하여 바라보고,
                            
                            StartCoroutine(Attack(livingEntity)); // 공격을 시도한다
                            yield break;
                        }
                    }
                }
                // Enemy가 움직이는 속도(velocity)의 크기(magnitude)를 이용하여,
                // 움직이는 애니메이션 처리를 한다.
                if (anim) anim.SetFloat("Magnitude", agent.velocity.magnitude);
            } // if(agent)
            yield return new WaitForSeconds(0.04f);
        } // While()
    } // UpdatePath(


    private IEnumerator Attack(LivingEntity target)
    {
        if (agent && target)
        {

            var trTarget = target.transform; // target의 Transform.
            while (!isDead && !target.isDead) // true에서 (!isDead && !target.isDead)로 변경.
            //while (true)
            {
                // 공격 모션 실행.
                if (anim) anim.SetTrigger("Attack");
                yield return new WaitForSeconds(1.1f);
                // 피격 판정 타이밍에 target이 유효한 거리에 있는지 확인.
                if (Vector3.Distance(trTarget.position, transform.position) > agent.stoppingDistance) break;
                // TODO : Player Damageable Code 추가.

                if (isDead || target.isDead) yield break;
                var hitNormal = transform.position - trTarget.position;
                target.OnDamage(damage, Vector3.zero, hitNormal);


                yield return new WaitForSeconds(1.2f);
                // 모션 종료 후, target이 유효한 거리에 있는지 확인.

                if (Vector3.Distance(trTarget.position, transform.position) > agent.stoppingDistance) break;
            }
        }
        // target과의 거리가 벌어진다면 다시 target을 쫓아 간다.
        if (!isDead) StartCoroutine(UpdatePath()); // if(!isDead) 조건 추가.
    } // Attack()

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        if (anim && !isDead)
        {
            if (hitEffect)
            {
                var hitEffectTR = hitEffect.transform;
                hitEffectTR.position = hitPoint; // 이펙트를 피격 지점으로 이동.
                                                 // 피격 당한 방향으로 회전.
                hitEffectTR.rotation = Quaternion.LookRotation(hitNormal);
                hitEffect.Play(); // 이펙트 재생.
            }
            // 피격 효과음 1회 재생.
            if (audioSource && hitSound) audioSource.PlayOneShot(hitSound);

            anim.SetTrigger("Damaged"); // 데미지를 입고 죽지 않았다면, 피격 애니메이션 실행.
        }
    }
    public void UnactiveObject() // Zombie Death 실행 후 호출하여 오브젝트를 비활성화 시킨다.
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


