using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 4f;
    private PlayerInput playerInput;
    private Rigidbody rigid;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody>();
        if (rigid) rigid.constraints = RigidbodyConstraints.FreezeRotation;
        anim = GetComponent<Animator>();
    }

    private void Move()
    {
        if (playerInput && rigid)
        {
            Vector3 velocity = transform.forward * playerInput.move * speed;
            rigid.velocity = velocity; //¶Ç´Â, rigid.MovePosition(rigid.position + velocity * 0.01f);
        }
    }
    private void Rotate()
    {
        if (playerInput && rigid)
        {
            float angle = playerInput.rotate * speed;
            rigid.rotation *= Quaternion.Euler(0, angle, 0);
        }
    }
    void FixedUpdate()
    {
        Move();
        Rotate();
        if (anim && playerInput) anim.SetFloat("Move", playerInput.move);
    }

}
