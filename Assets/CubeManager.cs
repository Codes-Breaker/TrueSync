using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    // Start is called before the first frame update

    public InputReaderBase inputReader;

    private Vector2 axisInput;
    private float maxActorGas = 100;
    public float currentGas;
    [HideInInspector]
    public bool releasing = false;
    private bool charge;
    public Rigidbody ridbody;
    public float chargeTime;
    public float releaseTime;
    public float releaseSpeedAtFirstArgument;
    public float releaseSpeedLinearArgument;
    private float movementThrashold = 0.01f;
    public Vector3 velocityBeforeCollision = Vector3.zero;
    public Vector3 positionBeforeCollision = Vector3.zero;
    public bool isWalk;
    public float froceArgument;
    void Start()
    {
        
    }

    private void Update()
    {
        //input
        axisInput = inputReader.axisInput;
        charge = inputReader.charge;
    }

    private void FixedUpdate()
    {
        velocityBeforeCollision = GetComponent<Rigidbody>().velocity;
        positionBeforeCollision = GetComponent<Rigidbody>().position;

        MoveCharge();
        MoveRelease();
        MoveWalk();
    }
    public float targetAngle;
    private void MoveWalk()
    {
        if (axisInput.magnitude > movementThrashold && !releasing)
        {
            targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            isWalk = true;
        }
        else
        {
            isWalk = false;
        }
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0)), 0.1f);
        
    }

    int speedUpGas = 5;
    const int maxSpeedUpGas = 5;

    private void MoveRelease()
    {
        if (!charge || releasing)
        {
            if (currentGas > 0)
            {
                var releaseDir = ridbody.transform.forward;
                releaseDir = releaseDir.normalized;
                if (currentGas < maxActorGas / 20)
                {
                    currentGas = 0;
                    releasing = false;
                    speedUpGas = maxSpeedUpGas;
                }
                else
                {
                    if (speedUpGas >= 0)
                    {
                        var addSpeed = EaseOutCirc(currentGas / (maxActorGas)) * releaseSpeedAtFirstArgument;
                        ridbody.AddForce(releaseDir * addSpeed, ForceMode.Impulse);
                        speedUpGas--;
                    }
                    else
                    {
                        var addSpeed = EaseOutCirc(currentGas / (maxActorGas)) * releaseSpeedLinearArgument;
                        ridbody.AddForce(releaseDir * addSpeed, ForceMode.Impulse);
                    }
                    // currentGas = currentGas - (currentGas) / releaseTime * Time.fixedDeltaTime;
                    currentGas = currentGas - (maxActorGas) / releaseTime * Time.fixedDeltaTime;
                    currentGas = Mathf.Max(0, currentGas);
                    releasing = true;
                }
            }
        }
    }

    private float EaseOutCirc(float number)
    {
        return Mathf.Sqrt(1 - Mathf.Pow(number - 1, 2));
    }

    private void MoveCharge()
    {
        if (charge)
        {
            if (currentGas < maxActorGas && !releasing)
            {
                // currentGas = currentGas + (maxActorGas - currentGas) / chargeTime * Time.fixedDeltaTime;
                currentGas = currentGas + maxActorGas / chargeTime * Time.fixedDeltaTime;
                currentGas = Mathf.Min(maxActorGas, currentGas);
                releasing = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CubeManager>())
        {
            var otherCollision = collision.gameObject.GetComponent<CubeManager>();

            Vector3 vel1 = velocityBeforeCollision;
            Vector3 vel2 = otherCollision.velocityBeforeCollision;

            Vector3 cPoint = collision.contacts[0].point;
            Vector3 contactToMe = cPoint - positionBeforeCollision;
            Vector3 contactToOther = cPoint - otherCollision.positionBeforeCollision;

            var d1 = Vector3.Angle(vel1, contactToMe);
            var d2 = Vector3.Angle(vel1, contactToOther);

            var degree1 = d1 * Mathf.Deg2Rad;
            var degree2 = d2 * Mathf.Deg2Rad;

            Vector3 impactVelocity = collision.relativeVelocity;

            var m1 = (Mathf.Cos(degree1) * vel1).magnitude;
            var m2 = (Mathf.Cos(degree2) * vel2).magnitude;

            if (m2 > m1)
            {
                Debug.LogError($"====>{froceArgument * m2} - {froceArgument} - {m2} ---> vel1 {vel1} vel2 {vel2}");
                ridbody.AddExplosionForce(froceArgument * m2, collision.contacts[0].point, 4);
                collision.collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce(froceArgument * m2, collision.contacts[0].point, 4);
            }
        }
    }


}
