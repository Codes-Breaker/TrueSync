using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class Controller : MonoBehaviour
{
    private TSRigidBody body;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        body = this.GetComponent<TSRigidBody>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (body.velocity.magnitude >= 20)
        //    return;
        if (Input.GetKey(KeyCode.A))
        {
            body.AddForce(new TSVector(-1f, 0, 0), ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.D))
        {
            body.AddForce(new TSVector(0.5f, 0, 0), ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.W))
        {
            body.AddForce(new TSVector(0, 0, 0.5f), ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.S))
        {
            body.AddForce(new TSVector(0, 0, -0.5f), ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            body.AddForce(new TSVector(0, 0, 500f), ForceMode.Impulse);
        }
    }
}
