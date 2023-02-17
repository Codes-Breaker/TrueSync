using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionStun : MonoBehaviour
{
    public float fallTime = 0;
    public float maxFallTime = 3;
    public bool fall;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > 50)
        {
            fall = true;
            fallTime = 0;
            SetBalance(0, 0);
        }
    }

    private void FixedUpdate()
    {
        if (fall)
        {
            fallTime += Time.fixedDeltaTime;
            if (fallTime >= maxFallTime)
            {
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
