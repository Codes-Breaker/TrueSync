using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDemo : MonoBehaviour
{
    public CollisionEffect collisionEffect;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            Debug.Log("First point that collided: " + collision.contacts[0].point);
            collisionEffect.AddCollision(collision.contacts[0].point, 1f, 1f);
        }

    }
}
