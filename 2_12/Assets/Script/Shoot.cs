using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;       //시간 사용을 위함

public class Shoot : MonoBehaviour
{
    public float speed = 0.0f;
    private Rigidbody rb;

    private float movementX = 0.0f;
    private float movementY = 0.0f;

    private float f_click = 0.0f;

    Vector3 mousePos, transPos, targetPos;

    Vector3 m_Pos, t_Pos, ta_Pos;

    private float gak = 0.0f;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
         if (Input.GetMouseButtonDown(0))
         {
            //rb.Sleep();
            //rb.gameObject.SetActive(true);

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            f_click = 0.0f;
      
            CalTrgetPos();
            MoveToTarget();

        }

        if (Input.GetMouseButton(0)) //좌클릭 동안
        {
            //Vector3 movement = new Vector3(-50.0f, movementX, movementY);
            //rb.AddForce(movement * speed, ForceMode.Impulse);
            //f_click = 0.0f;
            RotateToTarget();
            f_click += Time.deltaTime;
            
        }
        
        else if(Input.GetMouseButtonDown(1))
        {
            rb.gameObject.SetActive(false);
        }
        

        if(Input.GetMouseButtonUp(0))   //좌클릭 뗄시
        {
            Debug.Log(Input.mousePosition);

            if (f_click > 10.0f) //최대 speed 5로 제한
            {
                f_click = 10.0f;
            }
            speed = f_click*300;
            Debug.Log(speed);

            //Vector3 movement = new Vector3(-50.0f, movementX, movementY);
            Vector3 movement = new Vector3(-50.0f, movementX, movementY);
            //rb.AddForce(movement * speed, ForceMode.Impulse);
            rb.AddForce(transform.forward * speed, ForceMode.Impulse);

        }
    }

    void CalTrgetPos()
    {
        mousePos = Input.mousePosition;
        //mousePos.z = 47.7f; //카메라 위치 - 스틱 위치 = 1648 - 60
        mousePos.z = 1600.0f;
        transPos = Camera.main.ScreenToWorldPoint(mousePos);
        targetPos = new Vector3(transPos.x, transPos.y, transPos.z);
        Debug.Log("클릭1: " + targetPos);
    }

    void MoveToTarget()
    {
        //transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime);
        transform.position = targetPos;
    }

    void RotateToTarget()
    {
        m_Pos = Input.mousePosition;
        m_Pos.z = 1588.0f;
        t_Pos = Camera.main.ScreenToWorldPoint(m_Pos);
        ta_Pos = new Vector3(t_Pos.x, t_Pos.y, t_Pos.z);

        gak = GetAngle();
        Debug.Log("각도 : " + GetAngle());

        transform.rotation = Quaternion.Euler(0, gak, 0);
        //movementX = gak;
    }

    float GetAngle()
    {
        //Debug.Log(ta_Pos + "-----" + transPos);

        Vector3 angle = transPos - ta_Pos;

        Debug.Log(angle);
        return Mathf.Atan2(angle.x, angle.z) * Mathf.Rad2Deg;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("FBall"))
        {

            //rb.gameObject.SetActive(false);
            rb.transform.position = new Vector3(5000, 5000, 5000);
        }
    }

}
