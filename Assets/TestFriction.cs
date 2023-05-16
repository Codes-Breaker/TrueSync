using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFriction : MonoBehaviour
{
    Rigidbody body;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            body.AddExplosionForce(1000, this.transform.position - this.transform.forward, 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
