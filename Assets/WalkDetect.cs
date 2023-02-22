using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkDetect : MonoBehaviour
{
    public AnimalController controller;
    public CharacterManager characterController;
    public Grab grab;
    public Animator animator;

    public void Update()
    {
        if(controller)
            animator.SetBool("walk", controller.isWalk);
        if (characterController)
        {
            animator.SetBool("walk", characterController.isWalk);
            animator.SetBool("swing", characterController.swinging);
            animator.SetBool("readyswing", characterController.readyswing);
        }
        if (grab)
            animator.SetBool("eat", grab.eat);
    }
}
