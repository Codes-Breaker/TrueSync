using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocitySet : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rb;
    void Start()
    {
        
    }

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = Vector3.zero;
    }
}
