using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionStun : MonoBehaviour
{
    public float fallTime = 0;
    public float maxFallTime = 2;
    public bool fall;
    public ConfigurableJoint body;
    public Vector3 velocityBeforeCollision = Vector3.zero;
    public Vector3 positionBeforeCollision = Vector3.zero;
    public GameObject stunEffect;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.GetComponent<CollisionStun>() == null || collision.transform.gameObject.GetComponent<CollisionStun>() == this)
            return;

        Vector3 vel1 = velocityBeforeCollision;
        Vector3 vel2 = collision.transform.gameObject.GetComponent<CollisionStun>().velocityBeforeCollision;

        Vector3 cPoint = collision.contacts[0].point;
        Vector3 contactToMe = cPoint - positionBeforeCollision;
        Vector3 contactToOther = cPoint - collision.transform.gameObject.GetComponent<CollisionStun>().positionBeforeCollision;

        var d1 = Vector3.Angle(vel1, contactToMe);
        var d2 = Vector3.Angle(vel1, contactToOther);

        var degree1 = d1 * Mathf.Deg2Rad;
        var degree2 = d2 * Mathf.Deg2Rad;

        var m1 = (Mathf.Cos(degree1) * vel1).magnitude * body.GetComponent<Rigidbody>().mass;
        var m2 = (Mathf.Cos(degree2) * vel2).magnitude * collision.rigidbody.mass;


        if (m2 > 20)
        {
            fall = true;
            fallTime = 0;
            maxFallTime = 1;
            body.targetRotation = Quaternion.Euler( body.transform.rotation.eulerAngles.x, body.transform.rotation.eulerAngles.y, body.transform.rotation.eulerAngles.z);
            SetBalance(0, 0);
            Debug.LogWarning($"{this.gameObject.name} 别人对我的力 {m2} 对方的角度 {d2}");


        }
        else if (m2 > 15)
        {
            fall = true;
            fallTime = 0;
            maxFallTime = 0.5f;
            body.targetRotation = Quaternion.Euler(body.transform.rotation.eulerAngles.x, body.transform.rotation.eulerAngles.y, body.transform.rotation.eulerAngles.z);
            SetBalance(0, 0);
            Debug.LogWarning($"{this.gameObject.name} 别人对我的力 {m2} 对方的角度 {d2}");


        }
        else if (m2 > 10)
        {
            fall = true;
            fallTime = 0;
            maxFallTime = 0.25f;
            body.targetRotation = Quaternion.Euler(body.transform.rotation.eulerAngles.x, body.transform.rotation.eulerAngles.y, body.transform.rotation.eulerAngles.z);
            SetBalance(0, 0);
            Debug.LogWarning($"{this.gameObject.name} 别人对我的力 {m2}  对方的角度 {d2}");


        }
    }

    static Vector3 ComputeIncidentVelocity(Rigidbody body, Collision collision, out Vector3 otherVelocity)
    {
        Vector3 impulse = collision.impulse;
        // Both participants of a collision see the same impulse, so we need to flip it for one of them.
        if (Vector3.Dot(collision.GetContact(0).normal, impulse) < 0f)
            impulse *= -1f;
        otherVelocity = Vector3.zero;
        // Static or kinematic colliders won't be affected by impulses.
        var otherBody = collision.rigidbody;
        if (otherBody != null)
        {
            otherVelocity = otherBody.velocity;
            if (!otherBody.isKinematic)
                otherVelocity += impulse / otherBody.mass;
        }
        return body.velocity - impulse / body.mass;
    }

    private void FixedUpdate()
    {
        Vector3 vel1 = body.GetComponent<Rigidbody>().velocity;
        velocityBeforeCollision = vel1;
        positionBeforeCollision = body.GetComponent<Rigidbody>().position;
        if (fall)
        {
            stunEffect.gameObject.SetActive(true);
            fallTime += Time.fixedDeltaTime;
            if (fallTime >= maxFallTime)
            {
                stunEffect.gameObject.SetActive(false);
                fall = false;
                fallTime = 0;
                SetBalance(100, 300);
            }
        }
    }

    private void SetBalance(float x, float yz)
    {
        var cjs = this.transform.GetComponentsInChildren<ConfigurableJoint>();
        foreach(var cj in cjs)
        {
            var jointDriveX = new JointDrive()
            {
                positionSpring = x,
                positionDamper = cj.angularXDrive.positionDamper,
                maximumForce = cj.angularXDrive.maximumForce,
            };

            var jointDriveYZ = new JointDrive()
            {
                positionSpring = yz,
                positionDamper = cj.angularYZDrive.positionDamper,
                maximumForce = cj.angularYZDrive.maximumForce,
            };

            cj.angularXDrive = jointDriveX;
            cj.angularYZDrive = jointDriveYZ;
        }
    }


}
