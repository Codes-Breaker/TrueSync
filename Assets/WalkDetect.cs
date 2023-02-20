using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkDetect : MonoBehaviour
{
    public AnimalController controller;
    public CharacterManager characterController;
    public Animator animator;

    public void Update()
    {
        if(controller)
            animator.SetBool("walk", controller.isWalk);
        if (characterController)
            animator.SetBool("walk", characterController.isWalk);
    }
}
