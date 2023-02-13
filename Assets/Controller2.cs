using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class Controller2 : MonoBehaviour
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
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            body.AddForce(new TSVector(-1f, 0, 0), ForceMode.Impulse);
            Debug.LogWarning($"【FORCE】add force!!!!!!!!!");
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            body.AddForce(new TSVector(1f, 0, 0), ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            body.AddForce(new TSVector(0, 0, 1f), ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            body.AddForce(new TSVector(0, 0, -0.5f), ForceMode.Impulse);
        }
    }
}
