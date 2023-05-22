using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(ConfigurableJoint))]
public class CollisionStun : MonoBehaviour
{
    public float fallTime = 0;
    public float maxFallTime = 2;
    public bool fall;
    public ConfigurableJoint body;
    public Rigidbody rigidbody;
    public VelocityRecorder velRecorder;
    public Vector3 velocityBeforeCollision => velRecorder.velocityBeforeCollision;
    public Vector3 positionBeforeCollision => velRecorder.positionBeforeCollision;
    public GameObject stunEffect;
    public ConfigurableJoint[] cjs;
    public float originalDriveX;
    public float originalDriveY;
    public CharacterContorl characterManager;
    public ConfigurableJoint cj;
    public float minDriveX = 0;
    public float minDriveY = 0;
    public float froceArgument;
    private Quaternion configurableJointRotasion;

    private void Awake()
    {
        originalDriveX = cj.angularXDrive.positionSpring;
        originalDriveY = cj.angularYZDrive.positionSpring;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var otherCollision = collision.transform.gameObject.GetComponent<VelocityRecorder>();
        if (otherCollision == null)
            return;

        Vector3 vel1 = velocityBeforeCollision;
        Vector3 vel2 = otherCollision.velocityBeforeCollision;

        Vector3 cPoint = collision.contacts[0].point;
        Vector3 contactToMe = cPoint - positionBeforeCollision;
        Vector3 contactToOther = cPoint - otherCollision.positionBeforeCollision;

        var d1 = Vector3.Angle(vel1, contactToMe);
        var d2 = Vector3.Angle(vel1, contactToOther);

        var degree1 = d1 * Mathf.Deg2Rad;
        var degree2 = d2 * Mathf.Deg2Rad;

        Vector3 impactVelocity = collision.relativeVelocity;

        var m1 = (Mathf.Cos(degree1) * vel1).magnitude * rigidbody.mass;
        var m2 = (Mathf.Cos(degree2) * vel2).magnitude * otherCollision.rigidbody.mass;
        Debug.Log($"{this.gameObject.name} ���˶��ҵ��� {m2} �Է��ĽǶ� {d2} impulse {collision.impulse} impulse force sum {collision.relativeVelocity}");
        

        if (m2 > 7)
        {
            fall = true;
            fallTime = 0;
            maxFallTime = m2/2;
            body.targetRotation = Quaternion.Euler( body.transform.rotation.eulerAngles.x, body.transform.rotation.eulerAngles.y, body.transform.rotation.eulerAngles.z);
            configurableJointRotasion = cj.targetRotation;
            SetBalance(minDriveX, minDriveY);
           // rigidbody.AddExplosionForce(froceToOtherArgument * m2, collision.contacts[0].point, 4);

        }
        else if (m2 > 4)
        {
            fall = true;
            fallTime = 0;
            maxFallTime = m2 / 2;
            body.targetRotation = Quaternion.Euler(body.transform.rotation.eulerAngles.x, body.transform.rotation.eulerAngles.y, body.transform.rotation.eulerAngles.z);
            configurableJointRotasion = cj.targetRotation;
            SetBalance(minDriveX, minDriveY);
           // rigidbody.AddExplosionForce(froceToOtherArgument * m2, collision.contacts[0].point, 4);

        }
        else if (m2 > 2)
        {
            fall = true;
            fallTime = 0;
            maxFallTime = m2 / 2;
            body.targetRotation = Quaternion.Euler(body.transform.rotation.eulerAngles.x, body.transform.rotation.eulerAngles.y, body.transform.rotation.eulerAngles.z);
            configurableJointRotasion = cj.targetRotation;
            SetBalance(minDriveX, minDriveY);
           // rigidbody.AddExplosionForce(froceToOtherArgument * m2, collision.contacts[0].point, 4);

        }
        if(m2 > m1)
        {
            rigidbody.AddExplosionForce(froceArgument * m2, collision.contacts[0].point, 4);
            collision.collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce(froceArgument * m2, collision.contacts[0].point, 4);
        }

        //if (m2 > 8)
        //{
        //    characterManager.currentHPValue = characterManager.currentHPValue - 20;
        //}
        //else if(m2 > 6) 
        //{
        //    characterManager.currentHPValue = characterManager.currentHPValue - 16;
        //}
        //else if(m2 > 4)
        //{
        //    characterManager.currentHPValue = characterManager.currentHPValue - 12;
        //}
        //else if(m2 > 2)
        //{
        //    characterManager.currentHPValue = characterManager.currentHPValue - 8;
        //}
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
        if (fall)
        {
            stunEffect.gameObject.SetActive(true);
            fallTime += Time.fixedDeltaTime;
            cj.targetRotation =  Quaternion.Euler(-configurableJointRotasion.eulerAngles);
            if (fallTime >= maxFallTime)
            {
                stunEffect.gameObject.SetActive(false);
                fall = false;
                fallTime = 0;
                SetBalance(originalDriveX, originalDriveY);
           //     SetCharacterJointBalance(3, 3);
            }
        }
    }

    private void SetBalance(float x, float yz)
    {
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
