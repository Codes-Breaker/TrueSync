using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbCollision : MonoBehaviour
{
    public AnimalController playerController;
    public CharacterManager characterManager;
    private void Start()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(playerController)
            playerController.isGrounded = true;
        if (characterManager)
            characterManager.isGrounded = true;
    }
}
