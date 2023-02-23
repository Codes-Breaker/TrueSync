using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityRecorder : MonoBehaviour
{
    public Vector3 velocityBeforeCollision = Vector3.zero;
    public Vector3 positionBeforeCollision = Vector3.zero;
    public Rigidbody rigidbody;
    void FixedUpdate()
    {
        velocityBeforeCollision = rigidbody.velocity;
        positionBeforeCollision = rigidbody.position;
    }
}
