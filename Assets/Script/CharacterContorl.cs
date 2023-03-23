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
    public CollisionStun collisionStun;
    public Rigidbody ridbody;
    public Collider bodyCollider;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public ConfigurableJoint mainConfigurableJoint;
    public float originalRadius;
    public float targetRadius;
    public Vector3 targetCenter;
    public Vector3 originalCenter;

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


    public bool swinging = false;
    public bool readyswing = false;

    private void Awake()
    {
        currentHPValue = maxHPValue;
        isGrounded = true;
        originalRadius = (bodyCollider as SphereCollider).radius;
        originalCenter = (bodyCollider as SphereCollider).center;
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
            SetGravity();
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
        if (collisionStun.fall)
            return;
        if (axisInput.magnitude > movementThrashold && !releasing)
        {
            targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            //Vector3 forceForward = Quaternion.Euler(-targetAngle, 0f, 0f) * Vector3.forward;
            //Vector3 rotasionForward = new Vector3(-axisInput.x, 0, axisInput.y);
            //if (releasing)
            //    ridbody.AddForce(new Vector3(forceForward.y, forceForward.x, forceForward.z) * brakeSpeedArgument);
            //else if (isGrounded)
            //{
            //    //   ridbody.AddForce(new Vector3(forceForward.y * movementSpeed, 0, forceForward.z * movementSpeed));
            //    ridbody.AddForce(new Vector3(forceForward.y * movementSpeed, 0, forceForward.z * movementSpeed), ForceMode.Impulse);
            //}
            isWalk = true;
        }
        else
        {
            isWalk = false;
        }
        mainConfigurableJoint.targetRotation = Quaternion.Slerp(mainConfigurableJoint.targetRotation, Quaternion.Euler(-targetAngle, 0, 0), 0.1f);
    }


    private void MoveJump()
    {
        if (collisionStun.fall)
            return;
        //if (jump && (isGrounded || isGrabWall))
        //{
        //    ridbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        //    //isGrounded = false;
        //}
    }

    private void MoveCharge()
    {
        if (collisionStun.fall)
            return;
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
        if (collisionStun.fall)
            return;
        if (!charge || releasing)
        {
            if (currentGas > 0)
            {
                var releaseDir = new Vector3(-ridbody.transform.forward.z, 0, ridbody.transform.forward.x);
                releaseDir = releaseDir.normalized;
                if (currentGas < maxActorGas / 20)
                {
                    currentGas = 0;
                    releasing = false;
                }
                else
                {
                    if (!releasing)
                    {
                        var addSpeed = EaseOutCirc(currentGas / (maxActorGas)) * releaseSpeedAtFirstArgument;
                        ridbody.AddForce(releaseDir * addSpeed,ForceMode.Impulse);
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
            collisionStun.maxFallTime = cureTime;
            currentHPValue = 0;
            isSwimmy = true;
            return false;
        }
        return true;
    }

    #endregion

    private void SetSlider()
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
