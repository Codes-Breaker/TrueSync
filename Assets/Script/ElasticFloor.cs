using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElasticFloor : MonoBehaviour
{
    public float elasticity = 0.2f;

    private Rigidbody ballRigidbody;

    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log("collisionEnter");
        if (collision.gameObject.GetComponent<CharacterContorl>())
        {

            Debug.Log("characterCollisionEnter");
            ballRigidbody = collision.collider.attachedRigidbody;
            // �������䷽��
            Vector3 inDirection = collision.contacts[0].point - ballRigidbody.worldCenterOfMass;

            // �����ٶ�
            float speed = Mathf.Max(Vector3.Dot(collision.gameObject.GetComponent<CharacterContorl>().velocityBeforeCollision, inDirection.normalized), 0);

            // ���㷴����
            Vector3 bounceForce = -inDirection.normalized * speed * elasticity;

            // Ӧ�÷�����
            ballRigidbody.AddForce(bounceForce, ForceMode.Impulse);
        }
    }
}
