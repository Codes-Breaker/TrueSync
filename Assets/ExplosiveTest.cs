using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.rigidbody.AddExplosionForce(1000, collision.contacts[0].point, 10);
        }
    }
}
