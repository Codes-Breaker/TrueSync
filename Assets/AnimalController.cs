using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{

    public float speed;
    public float jumpForce;

    public Rigidbody body;
    public bool isGrounded;
    public bool releasing = false;
    public float maxScale;

    public ConfigurableJoint neckPoint;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            body.AddForce(body.transform.right * speed);
        }

        if (Input.GetKey(KeyCode.A))
        {
            body.AddForce(body.transform.forward * speed);
        }

        if (Input.GetKey(KeyCode.S))
        {
            body.AddForce(-body.transform.right * speed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            body.AddForce(-body.transform.forward * speed);
        }

        if (Input.GetAxis("Jump") > 0)
        {
            if (isGrounded)
            {
                body.AddForce(new Vector3(0, jumpForce, 0));
                isGrounded = false;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (body.transform.localScale.x < maxScale)
            {
                body.transform.localScale = Vector3.Lerp(body.transform.localScale, new Vector3(maxScale, maxScale, maxScale), 0.1f);
                neckPoint.targetRotation = Quaternion.Euler(0, 0, -45);
            }
        }
        else
        {
            if (body.transform.localScale.x > 1.0f)
            {
                if (body.transform.localScale.x - 1.0f < 0.02f)
                {
                    body.transform.localScale = new Vector3(1, 1, 1);
                    releasing = false; 
                }
                else if (body.transform.localScale.x > 1.29f)
                {
                    if (!releasing)
                    {
                        body.AddForce(body.transform.right * speed * 100.0f);
                    }
                    else
                    {
                        body.AddForce(body.transform.right * speed * 2.0f);
                    }
                    body.transform.localScale = Vector3.Lerp(body.transform.localScale, new Vector3(1.0f, 1.0f, 1.0f), 0.01f);
                    neckPoint.targetRotation = Quaternion.Euler(0, 0, 0);

                    releasing = true;
                }
                else
                {
                    body.transform.localScale = Vector3.Lerp(body.transform.localScale, new Vector3(1.0f, 1.0f, 1.0f), 0.01f);
                    neckPoint.targetRotation = Quaternion.Euler(0, 0, 0);
                    body.AddForce(body.transform.right * speed * 2.0f);
                }
            }

        }



    }
}
