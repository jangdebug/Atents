using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ball_hit : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public GameObject ballShoot;
    private Rigidbody rb;
    private int count;
    private int one_count;
    private int two_count;
    private int rival_count;

     

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        one_count = 0;
        two_count = 0;
        //Debug.Log(name);
        SetCountText();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude < 12.0f)// new Vector3(0.05f, 0.05f, 0.05f))
        {
            if (one_count == 1 && two_count == 1 && rival_count != 1)
            {
                count = count + 1;
            }
            else if(rival_count == 1 && count != 0)
            {
                count = count - 1;
            }
            one_count = 0;
            two_count = 0;
            rival_count = 0;

            rb.velocity = Vector3.zero;

            ballShoot.SetActive(true);
            SetCountText();         
        }
        else
        {
            ballShoot.SetActive(false);
        }
    }

    void SetCountText()
    {
        countText.text = "Count : " + count.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {        

        //if((one_count == 1 && collision.gameObject.CompareTag("HBall2")) || 
        //    (two_count == 1 && collision.gameObject.CompareTag("HBall")) &&
        //    !collision.gameObject.CompareTag("RivalBall"))
        //{
        //    count = count + 1;
        //}
        //else

        if (collision.gameObject.CompareTag("HBall"))
        {
            one_count = one_count + 1;
        }

        else if (collision.gameObject.CompareTag("HBall2"))
        {
            two_count = two_count + 1;
        }

        else if (collision.gameObject.CompareTag("RivalBall"))
        {
            //if(count != 0)
            //{
            //    count = count - 1; 
            //}
            rival_count = rival_count + 1;
        }
      

      
        
    }
}
