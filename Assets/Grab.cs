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
    public string WeaponTag;
    [SerializeField]
    public string[] tags;
    public InputReaderBase inputReader;
    public CharacterManager characterController;
    public Weapon weapon;
    public CollisionStun stun;
    public Transform bindPoint;
    FixedJoint fj;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Drop()
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
        if (weapon != null && weapon.controller == characterController)
        {
            weapon.OnUnEquipped();
            weapon.controller = null;
            weapon.transform.parent = null;
        }
        eat = false;
        weapon = null;
        alreadyGrabbing = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (stun.fall)
        {
            Drop();
        }
        else
        {
            if (inputReader.pull)
            {
                if (grabbedObj != null && !alreadyGrabbing)
                {
                    bool ownWeapon = false;
                    eat = true;
                    if (grabbedObj.gameObject.CompareTag(WeaponTag))
                    {
                        weapon = grabbedObj.GetComponent<Weapon>();
                        if (weapon.controller == null)
                        {
                            weapon.controller = characterController;
                            if (bindPoint && weapon.canGrabInMouth)
                            {
                                weapon.transform.parent = bindPoint;
                                grabbedObj.transform.position = bindPoint.transform.position;
                            }
                            weapon.OnEquipped();
                            ownWeapon = true;
                        }

                        if (!weapon.canGrabInMouth)
                        {
                            fj = grabbedObj.AddComponent<FixedJoint>();
                            fj.connectedBody = rb;
                            fj.breakForce = 1000;
                        }
                    }
                    else
                    {
                        fj = grabbedObj.AddComponent<FixedJoint>();
                        fj.connectedBody = rb;
                        fj.breakForce = 1000;
                    }

                    alreadyGrabbing = true;
                    if (ownWeapon)
                    {
                        weapon = grabbedObj.GetComponent<Weapon>();
                        //weapon.fixJoint = fj;
                    }
                }
                else
                {
                    eat = false;
                }
            }
            else
            {
                Drop();
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        foreach(var item in tags)
        {
            if (other.gameObject.CompareTag(item))
            {
                grabbedObj = other.gameObject;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        foreach (var item in tags)
        {
            if (other.gameObject.CompareTag(item))
            {
                grabbedObj = other.gameObject;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        grabbedObj = null;
    }


}
