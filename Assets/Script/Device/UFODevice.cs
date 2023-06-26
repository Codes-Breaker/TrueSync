using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFODevice : MonoBehaviour
{
    [Header("向上吸力参数")]
    public AnimationCurve suctionToTargetForceParameters;

    [Header("水平吸力参数")]
    public AnimationCurve suctionToCenterForceParameters;

    [Header("模拟空气阻力")]
    public float frictionCoefficient = 0.6f;
    [Header("吸力终点")]
    public Transform suctionTarget;

    [Header("最小控制距离")]
    public float minDistance;

    [Header("是否有吸力")]
    public bool isSuction;

    private void OnTriggerStay(Collider other)
    {
        if(isSuction)
        {
            var otherRB = other.GetComponent<Rigidbody>();
            if (otherRB)
            {
                var otherPositon =  other.transform.position;
                var selfPositon = suctionTarget.position;
                var distance = (otherPositon - selfPositon).magnitude;
                var target = (selfPositon - otherPositon).normalized;
                otherRB.AddForce(Vector3.up * suctionToTargetForceParameters.Evaluate(distance) * otherRB.mass, ForceMode.Force);
                otherRB.AddForce(Vector3.ProjectOnPlane(target, Vector3.up).normalized * suctionToCenterForceParameters.Evaluate(distance) * otherRB.mass, ForceMode.Force);
                // 计算空气阻力
                Vector3 airResistance = - otherRB.velocity * frictionCoefficient;
                // 应用空气阻力
                otherRB.AddForce(Vector3.ProjectOnPlane(airResistance , Vector3.up) * otherRB.mass);
            }


        }
    }
}
