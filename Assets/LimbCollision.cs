using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbCollision : MonoBehaviour
{
    public AnimalController playerController;
    private void Start()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        playerController.isGrounded = true;
    }
}
