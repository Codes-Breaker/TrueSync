using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{

    public float speed;
    public float jumpForce;
    public bool isWalk;
    public Rigidbody body;
    public bool isGrounded;
    public bool releasing = false;
    public float minScale;
    public float maxScale;
    public Transform releaseEffect;
    public ConfigurableJoint neckPoint;
    public Transform cameraFollowPoint;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var point = new Vector3(body.transform.position.x, body.transform.position.y, body.transform.position.z);
    }

    public float Cal(float x)
    {
        return -(Mathf.Cos(Mathf.PI * x) - 1) / 2;
    }

    private void FixedUpdate()
    {
        var vector3Move = new Vector3(-Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        var upRotation = new Vector3(1, 0, 0);
        var downRotation = new Vector3(0, -1, 0);
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            body.gameObject.GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Slerp(body.gameObject.GetComponent<ConfigurableJoint>().targetRotation, Quaternion.LookRotation(vector3Move), 0.1f);
        }


        if (Input.GetKey(KeyCode.W))
        {
            body.AddForce(new Vector3(0, 0, 1) * speed);
        }

        if (Input.GetKey(KeyCode.A))
        {
            body.AddForce(new Vector3(-1, 0, 0) * speed);
        }

        if (Input.GetKey(KeyCode.S))
        {
            body.AddForce(new Vector3(0, 0, -1) * speed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            body.AddForce(new Vector3(1, 0, 0) * speed);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            isWalk = true;
        }
        else
        {
            isWalk = false;
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
            if (body.transform.localScale.x < maxScale && !releasing)
            {
                body.transform.localScale = Vector3.Lerp(body.transform.localScale, new Vector3(maxScale, maxScale, maxScale), 0.1f);
                //neckPoint.targetRotation = Quaternion.Euler(0, 0, -45);
                releasing = false;
            }
        }
        else
        {
            var releaseDir = new Vector3(body.transform.right.x, 0, body.transform.right.z);
            if (body.transform.localScale.x > minScale)
            {
                if (body.transform.localScale.x - minScale < 0.02f)
                {
                    body.transform.localScale = new Vector3(minScale, minScale, minScale);
                    releasing = false;
                    //releaseEffect.gameObject.SetActive(false);
                }
                else if (body.transform.localScale.x > 1f)
                {
                    if (!releasing)
                    {
                        var addSpeed = (body.transform.localScale.x / (maxScale * 1f)) * 120f;
                        body.AddForce(releaseDir * speed * addSpeed);
                    }
                    else
                    {
                        body.AddForce(releaseDir * speed * 2.0f);
                    }
                    body.transform.localScale = Vector3.Lerp(body.transform.localScale, new Vector3(minScale, minScale, minScale), 0.01f);
                    //neckPoint.targetRotation = Quaternion.Euler(0, 0, 0);
                    //releaseEffect.gameObject.SetActive(true);
                    releasing = true;
                }
                else
                {
                    body.transform.localScale = Vector3.Lerp(body.transform.localScale, new Vector3(minScale, minScale, minScale), 0.01f);
                    //neckPoint.targetRotation = Quaternion.Euler(0, 0, 0);
                    body.AddForce(releaseDir * speed * 2.0f);
                }
            }

        }



    }
}
