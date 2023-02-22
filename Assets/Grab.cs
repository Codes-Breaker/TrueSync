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
    public CharacterManager characterController;
    public Weapon weapon;
    public CollisionStun stun;
    public Transform bindPoint;
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
                if (weapon != null && weapon.controller == characterController)
                {
                    weapon.OnUnEquipped();
                    weapon.controller = null;
                    weapon.transform.parent = null;
                }
                weapon = null;
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
                    bool ownWeapon = false;
                    eat = true;
                    if (grabbedObj.gameObject.CompareTag(WeaponTag))
                    {
                        weapon = grabbedObj.GetComponent<Weapon>();
                        if (weapon.controller == null)
                        {
                            weapon.controller = characterController;
                            weapon.transform.parent = bindPoint;
                            weapon.OnEquipped();
                            ownWeapon = true;
                        }
                        if (bindPoint)
                        {
                            grabbedObj.transform.position = bindPoint.transform.position;
                        }
                    }
                    fj = grabbedObj.AddComponent<FixedJoint>();
                    fj.connectedBody = rb;
                    fj.breakForce = 1000;
                    alreadyGrabbing = true;
                    if (ownWeapon)
                    {
                        weapon = grabbedObj.GetComponent<Weapon>();
                        weapon.fixJoint = fj;

                    }
                }
                else
                {
                    eat = false;
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
