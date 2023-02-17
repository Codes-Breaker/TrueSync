using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkDetect : MonoBehaviour
{
    public AnimalController controller;
    public Animator animator;

    public void Update()
    {
        animator.SetBool("walk", controller.isWalk);
    }
}
