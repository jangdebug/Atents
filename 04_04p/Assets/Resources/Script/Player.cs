using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

public class Player : MonoBehaviour
{

    private Rigidbody rigid;
    [SerializeField] private float speed = 8f;
    private Vector3 velocity = Vector3.zero;
    public Vector3 position { get { return transform.position; } }

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.constraints = RigidbodyConstraints.FreezePositionY 
            | RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationZ;
    }

    // Update is called once per frame
    //void Update(){}

    private void FixedUpdate()
    {
        velocity.x = Input.GetAxis("Horizontal") * speed;
        velocity.z = Input.GetAxis("Vertical") * speed;
        rigid.velocity = velocity;
    }

    public bool isLive { get { return gameObject.activeSelf; } }

    public void OnDamaged()
    {
        gameObject.SetActive(false);
    }

    public void Init()
    {
        velocity = Vector3.zero;
        gameObject.SetActive(true);
    }


}
