using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // Settings
    public float MoveSpeed = 50;
    public float MaxSpeed = 15;
    public float Drag = 0.98f;
    public float SteerAngle = 20;
    public float Traction = 1;

    // Variables
    private Vector3 MoveForce;

    // Update is called once per frame
    void Update()
    {
        // forward
        var forward = new Vector3(transform.forward.x, 0, transform.forward.z);

        // Moving
        MoveForce += forward * MoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;

        if (GetComponent<Animator>())
        {
            if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f)
            {
                GetComponent<Animator>().SetBool("run", true);
            }
            else
            {
                GetComponent<Animator>().SetBool("run", false);
            }

        }

        transform.position += MoveForce * Time.deltaTime;

        // Steering
        float steerInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * steerInput * MoveForce.magnitude * SteerAngle * Time.deltaTime);

        // Drag and max speed limit
        MoveForce *= Drag;
        MoveForce = Vector3.ClampMagnitude(MoveForce, MaxSpeed);

        // Traction
        Debug.DrawRay(transform.position, MoveForce.normalized * 3);
        Debug.DrawRay(transform.position, transform.forward * 3, Color.blue);
        MoveForce = Vector3.Lerp(MoveForce.normalized, forward, Traction * Time.deltaTime) * MoveForce.magnitude;

    }
}
