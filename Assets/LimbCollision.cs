using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbCollision : MonoBehaviour
{
    public AnimalController playerController;
    private void Start()
    {
        playerController = GameObject.FindObjectOfType<AnimalController>().GetComponent<AnimalController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        playerController.isGrounded = true;
    }
}
