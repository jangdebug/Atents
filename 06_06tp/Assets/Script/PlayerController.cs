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

    private void OnCollisionEnter2D(Collision2D collision)      //���� ���� ��
    {
        // normal : �浹 ������ ���� ����, �浹 ��󿡼� ���� ���ϴ� ���� ����.
        if (collision.contacts[0].normal.y > 0.8f)      //ù �浹�� y�� 0.8 �̻��� ��
                      // ������ ����(��) �κ��� y�� ���ʹ� 1, x�� ���ʹ� 0
                      // ������ ����(����) �κ��� y�� ���ʹ� 0, y�� ���ʹ� -1
                      // ĳ���Ͱ� ���� ���� ���� ���� y�� ���� 1�̴�.
                      // ������ float�� ����Ͽ��� ������ �ٻ�ġ 0.999....�� ���� �� �־�
                      // 1�� �ٻ�ġ�� 0.8�� ����� ���̴�.
        {
            if (anim) anim.SetBool("isGround", true);
            jumpCount = 0;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)       //������ ������ ��
    {
        if (anim) anim.SetBool("isGround", false);
    }


    private void OnTriggerEnter2D(Collider2D collision)     //ù �浹 ��, Ʈ���Źߵ�
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
