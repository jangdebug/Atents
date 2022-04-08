using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    
    Rigidbody2D rigid;
    Animator anim;

    [SerializeField] float jumpForce = 300f;
    readonly int limitJumpCount = 2;
    int jumpCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        if (rigid) rigid.freezeRotation = true;
        anim = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        if (GameMgr.Instance.isDead || !rigid) return;

        //if (!rigid) return;
        if (Input.GetKeyDown(KeyCode.Space) && limitJumpCount > jumpCount)
        {
            jumpCount++;
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpForce);

            SoundMgr.Instance.PlaySFX("jump");
        }
        else if (Input.GetKeyUp(KeyCode.Space) && 0 < rigid.velocity.y)
        {
            rigid.velocity *= 0.5f;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)      //땅에 닿을 때
    {
        // normal : 충돌 지점의 법선 벡터, 충돌 대상에서 나를 향하는 법선 벡터.
        if (collision.contacts[0].normal.y > 0.8f)      //첫 충돌이 y축 0.8 이상일 때
                      // 지형의 가로(땅) 부분의 y축 벡터는 1, x축 벡터는 0
                      // 지형의 세로(옆면) 부분의 y축 벡터는 0, y축 벡터는 -1
                      // 캐릭터가 땅에 있을 떄의 값은 y축 벡터 1이다.
                      // 하지만 float를 사용하였기 때문에 근사치 0.999....가 나올 수 있어
                      // 1의 근사치인 0.8을 사용한 것이다.
        {
            if (anim) anim.SetBool("isGround", true);
            jumpCount = 0;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)       //땅에서 떨어질 때
    {
        if (anim) anim.SetBool("isGround", false);
    }


    private void OnTriggerEnter2D(Collider2D collision)     //첫 충돌 시, 트리거발동
    {
        if (collision.tag.Equals("DeadZone"))
        {
            if (rigid) rigid.simulated = false;
            if (anim) anim.SetTrigger("IsDie");
            GameMgr.Instance.OnDie();

            SoundMgr.Instance.PlaySFX("die");

        }
    }

    private void GameOver()
    {
        GameMgr.Instance.GameOver();
    }

}
