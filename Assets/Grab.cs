using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject grabbedObj;
    public Rigidbody rb;
    public bool alreadyGrabbing = false;
    public string Tag;
    public InputReaderBase inputReader;
    public CollisionStun stun;
    FixedJoint fj;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stun.fall)
        {
            if (grabbedObj != null)
            {
                Destroy(fj);
                grabbedObj = null;
                fj = null;
                alreadyGrabbing = false;
            }
        }
        else
        {
            if (inputReader.pull)
            {
                if (grabbedObj != null && fj == null)
                {
                    fj = grabbedObj.AddComponent<FixedJoint>();
                    fj.connectedBody = rb;
                    fj.breakForce = 1000;
                    alreadyGrabbing = true;
                }

            }
            else
            {
                if (grabbedObj != null)
                {
                    Destroy(fj);
                    grabbedObj = null;
                    fj = null;
                    alreadyGrabbing = false;
                }
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Tag))
        {
            grabbedObj = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!alreadyGrabbing)
            grabbedObj = null;
    }


}
