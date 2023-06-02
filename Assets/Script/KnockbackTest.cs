using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackTest : MonoBehaviour
{
    public float explosiveForce = 20;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        collision.rigidbody.AddExplosionForce(explosiveForce, collision.contacts[0].point, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
