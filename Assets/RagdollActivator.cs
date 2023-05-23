using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollActivator : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody body;
    public Animator animator;
    public List<Collider> ragdollCollider;
    public GrounderQuadruped groundIK;
    public Rigidbody mainRag;
    void Start()
    {

    }

    public void Ragdoll(bool isRagdoll, Vector3 hitDir)
    {
        groundIK.weight = isRagdoll ? 0 : 1;
        animator.enabled = !isRagdoll;
        if (isRagdoll)
        {
            //StartCoroutine(disableMainCollider());
            //Destroy(body.GetComponent<Collider>());
            //Destroy(body);
            body.detectCollisions = false;
            body.isKinematic = true;

        }
        foreach (var col in ragdollCollider)
        {
            Physics.IgnoreCollision(body.GetComponent<Collider>(), col);
            col.GetComponent<Rigidbody>().isKinematic = !isRagdoll;
            col.GetComponent<Rigidbody>().detectCollisions = isRagdoll;
            //foreach(var col1 in ragdollCollider)
            //{
            //    Physics.IgnoreCollision(col1, col);
            //}
        }

        if (isRagdoll)
        {
            mainRag.AddForce(hitDir * 5000f);
        }
    }

    IEnumerator disableMainCollider()
    {
        yield return new WaitForSeconds(0.5f);
        body.isKinematic = true;
        body.useGravity = false;
        body.detectCollisions = false;
    }


    // Update is called once per frame
    void Update()
    {

    }
}
