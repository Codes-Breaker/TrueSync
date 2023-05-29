using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerList : MonoBehaviour
{
    public List<Rigidbody> affectingBodies;
    private void Awake()
    {
        affectingBodies = new List<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            this.affectingBodies.Add(other.attachedRigidbody);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            this.affectingBodies.Remove(other.attachedRigidbody);
        }
    }
}
