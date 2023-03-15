using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class IKTest : MonoBehaviour
{
    [Range(0,1)]
    public float weight = 1;
    public Animator animator;
    public Transform rightHandPoint;
    public Transform leftHandPoint;
    public bool right;
    public bool left;

    private void Update()
    {
    }


    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
        if (rightHandPoint && right)
        {
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPoint.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandPoint.rotation);
        }
        if (leftHandPoint && left)
        {
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPoint.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandPoint.rotation);
        }
    }



}
