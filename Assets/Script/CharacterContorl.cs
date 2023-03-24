using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterContorl : MonoBehaviour
{
    [Header("控制器")]
    public InputReaderBase inputReader;
    [Space(10)]
    [Header("参数")]
    public float movementSpeed;
    public float brakeSpeedArgument;
    public float jumpForce;
    public float releaseSpeedAtFirstArgument;
    public float releaseSpeedLinearArgument;
    [Tooltip("地面的Layers")]
    [SerializeField] public LayerMask groundMask;
    [Tooltip("Lerp计算参数")]
    public float chargeTime;
    [Tooltip("Lerp计算参数")]
    public float releaseTime;
    public float cureTime;

    //控制器相关
    private Vector2 axisInput;
    private bool jump;
    private bool charge;

    //摇杆输入值最小值
    private float movementThrashold = 0.01f;
    [Space(10)]
    [Header("相关需要关联组件")]
    public Slider hpSlider;
    public Slider gpSlider;
    public Canvas canvas;
    public Rigidbody ridbody;
    public Collider bodyCollider;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public float originalRadius;
    public float targetRadius;
    public Vector3 targetCenter;
    public Vector3 originalCenter;

    private Vector3 initialRotation;

    // 隐藏参数
    [HideInInspector]
    public bool isWalk;
    [HideInInspector]
    public bool isGrounded;
    [HideInInspector]
    public float targetAngle;
    private float maxHPValue = 100;
    [HideInInspector]
    public float currentHPValue;
    private float lastHPValue;
    private float maxActorGas = 100;
    [HideInInspector]
    public float currentGas;
    [HideInInspector]
    public bool releasing = false;
    [HideInInspector]
    public bool isSwimmy = false;
    [HideInInspector]
    public bool isGrabWall = false;
    [HideInInspector]
    public float HPtimer;
    
    public float froceArgument;

    public bool swinging = false;
    public bool readyswing = false;

    int speedUpGas = 0;
    public const int maxSpeedUpGas = 5;

    public Vector3 velocityBeforeCollision = Vector3.zero;
    public Vector3 positionBeforeCollision = Vector3.zero;

    public float maxReleaseVelocity;

    private void Awake()
    {
        speedUpGas = maxSpeedUpGas;
        currentHPValue = maxHPValue;
        isGrounded = true;
        originalRadius = (bodyCollider as SphereCollider).radius;
        originalCenter = (bodyCollider as SphereCollider).center;

        initialRotation = ridbody.transform.rotation.eulerAngles;
    }

    private void Update()
    {
        //input
        axisInput = inputReader.axisInput;
        jump = inputReader.jump;
        charge = inputReader.charge;
        //cameraInputX = inputReader.cameraInput.x;
        //MoveCamera();
    }

    private void LateUpdate()
    {
        SetSlider();
    }



    private void FixedUpdate()
    {
        velocityBeforeCollision = GetComponent<Rigidbody>().velocity;
        positionBeforeCollision = GetComponent<Rigidbody>().position;
        CheckIsGrounded();
        if (CheckHP())
        {
            MoveCharge();
            MoveRelease();
            MoveWalk();
            MoveJump();
        }
        SetState();
        if (!isGrounded)
        {
            //SetGravity();
        }
    }

    private void SetGravity()
    {
        ridbody.AddForce(Physics.gravity - Vector3.one, ForceMode.Acceleration);
    }


    #region Move

    private void SetState()
    {
        var gasScale = (currentGas / (maxActorGas * 1.0f));
        //skinnedMeshRenderer.SetBlendShapeWeight(0, gasScale * 100f);
        //(bodyCollider as SphereCollider).radius = Mathf.Lerp(originalRadius, targetRadius, gasScale);
    }
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
        this.ridbody.transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), 0.1f);
    }


    private void MoveJump()
    {
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

    private float EaseOutCirc(float number)
    {
        return Mathf.Sqrt(1 - Mathf.Pow(number - 1, 2));
    }



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

                    Vector3 vel1 = velocityBeforeCollision;

                    var d1 = Vector3.Angle(vel1, releaseDir);

                    var degree1 = d1 * Mathf.Deg2Rad;
                    var m1 = (Mathf.Cos(degree1) * vel1).magnitude;

                    if (speedUpGas >= 0)
                    {
                        var addSpeed = EaseOutCirc(currentGas / (maxActorGas)) * releaseSpeedAtFirstArgument;
                        if (m1 < maxReleaseVelocity)
                            ridbody.AddForce(releaseDir * addSpeed, ForceMode.Impulse);
                        speedUpGas--;
                    }
                    else
                    {
                        var addSpeed = EaseOutCirc(currentGas / (maxActorGas)) * releaseSpeedLinearArgument;
                        if (m1 < maxReleaseVelocity)
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

    #endregion

    #region Check

    private void CheckIsGrounded()
    {
        isGrounded = Physics.CheckSphere(bodyCollider.transform.position - new Vector3(0, (bodyCollider as SphereCollider).radius / 2, 0), 2f, groundMask);
    }


    private bool CheckHP()
    {

        if (currentHPValue == lastHPValue)
            HPtimer += Time.fixedDeltaTime;
        else
            HPtimer = 0;

        if (HPtimer > 15)
        {
            currentHPValue = currentHPValue + maxHPValue / cureTime * Time.fixedDeltaTime;
            currentHPValue = Mathf.Min(currentHPValue, maxHPValue);
            lastHPValue = currentHPValue;
        }
        else
        {
            lastHPValue = currentHPValue;
        }


        if (isSwimmy)
        {
            currentHPValue = currentHPValue + maxHPValue / cureTime * Time.fixedDeltaTime;
            currentHPValue = Mathf.Min(currentHPValue, maxHPValue);
            if (currentHPValue == maxHPValue)
            {
                isSwimmy = false;
            }
            return false;
        }

        if (currentHPValue <= 0)
        {
            currentHPValue = 0;
            isSwimmy = true;
            return false;
        }
        return true;
    }

    #endregion

    private void SetSlider()
    {
        if (hpSlider != null)
        {
            hpSlider.value = (float)(currentHPValue / maxHPValue);
            gpSlider.value = (float)(currentGas / maxActorGas);
            canvas.transform.forward = Camera.main.transform.forward;
            hpSlider.transform.position = bodyCollider.transform.position;
            gpSlider.transform.position = bodyCollider.transform.position;
            hpSlider.transform.localPosition = hpSlider.transform.localPosition + new Vector3(0, 1.7f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
            gpSlider.transform.localPosition = gpSlider.transform.localPosition + new Vector3(0, 1.3f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterContorl>())
        {
            var otherCollision = collision.gameObject.GetComponent<CharacterContorl>();

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

            Debug.LogError($"====>{froceArgument * m2} - {froceArgument} - {m2} ---> vel1 {vel1} vel2 {vel2}");
            ridbody.AddExplosionForce(froceArgument * m2, collision.contacts[0].point, 4);
            collision.collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce(froceArgument * m2, collision.contacts[0].point, 4);
            
        }
    }

}
