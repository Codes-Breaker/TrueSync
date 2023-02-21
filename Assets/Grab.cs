using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject grabbedObj;
    public Rigidbody rb;
    public bool alreadyGrabbing = false;
    public bool eat = false;
    public string Tag;
    public string WeaponTag;
    public InputReaderBase inputReader;
    public Weapon weapon;
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
                weapon = null;
                fj = null;
                alreadyGrabbing = false;
            }
        }
        else
        {
            if (inputReader.pull)
            {
                eat = true;
                if (grabbedObj != null && fj == null)
                {
                    fj = grabbedObj.AddComponent<FixedJoint>();
                    fj.connectedBody = rb;
                    fj.breakForce = 1000;
                    alreadyGrabbing = true;
                    if (grabbedObj.gameObject.CompareTag(WeaponTag))
                    {
                        weapon = grabbedObj.GetComponent<Weapon>();
                    }
                }
                else
                {
                    //eat = false;
                }
            }
            else
            {
                if (grabbedObj != null)
                {
                    grabbedObj = null;

                }
                if (fj != null)
                {
                    Destroy(fj);
                    fj = null;
                }
                eat = false;
                weapon = null;
                alreadyGrabbing = false;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Tag))
        {
            grabbedObj = other.gameObject;
        }
        else if (other.gameObject.CompareTag(WeaponTag))
        {
            grabbedObj = other.gameObject;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(Tag))
        {
            grabbedObj = other.gameObject;
        }
        else if (other.gameObject.CompareTag(WeaponTag))
        {
            grabbedObj = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        grabbedObj = null;
    }


}
