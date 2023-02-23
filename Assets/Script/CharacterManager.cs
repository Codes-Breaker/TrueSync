using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CharacterManager : MonoBehaviour
{
    [Header("控制器")]
    public InputReaderBase inputReader;
    [Space(10)]
    [Header("参数")]
    public float movementSpeed;
    public float jumpForce;
    public float releaseSpeed;
    public float maxScale;
    [Tooltip("地面的Layers")]
    [SerializeField] LayerMask groundMask;
    [Tooltip("Lerp计算参数")]
    public float chargeTime; 
    [Tooltip("Lerp计算参数")]
    public float releaseTime;
    public float cureTime;

    //控制器相关
    private Vector2 axisInput;
    private float cameraInputX;
    private bool jump;
    private bool charge;

    //摇杆输入值最小值
    private float movementThrashold = 0.01f;
    [Space(10)]
    [Header("相关需要关联组件")]
    public ConfigurableJoint neckPoint;
    public GameObject characterCamera;
    public GameObject otherCharacterCamera;
    public Transform cameraFollowPoint;
    public Slider hpSlider;
    public Slider gpSlider;
    public Canvas canvas;
    public Slider otherHpSlider;
    public Slider otherGpSlider;
    public Canvas otherCanvas;
    public Transform releaseEffect;
    public Grab grab;


    // 隐藏参数
    [HideInInspector]
    public bool isWalk;
    [HideInInspector]
    public bool isGrounded ;
    [HideInInspector]
    public float targetAngle;
    private float maxHPValue = 100;
    [HideInInspector]
    public float currentHPValue;
    private float maxActorGas = 100;
    [HideInInspector]
    public float currentGas;
    [HideInInspector]
    public bool releasing = false;
    [HideInInspector]
    public CollisionStun collisionStun;
    [HideInInspector]
    public bool isSwimmy = false;
    [HideInInspector]
    public bool isGrabWall = false;


    private float deltaScale;

    private Rigidbody ridbody;

    [HideInInspector]
    public bool swinging = false;
    [HideInInspector]
    public bool readyswing = false;

    private void Awake()
    {
        ridbody = GetComponent<Rigidbody>();
        currentHPValue = maxHPValue;
        deltaScale = (maxScale - 1) / maxActorGas;
        collisionStun = GetComponent<CollisionStun>();
        isGrounded = true;
    }

    private void Update()
    {
        //input
        axisInput = inputReader.axisInput;
        jump = inputReader.jump;
        charge = inputReader.charge;
        cameraInputX = inputReader.cameraInput.x;
        MoveCamera();
    }

    private void LateUpdate()
    {
        var point = new Vector3(ridbody.transform.position.x, ridbody.transform.position.y, ridbody.transform.position.z);
        cameraFollowPoint.transform.position = Vector3.Lerp(cameraFollowPoint.transform.position, point, 0.1f);
        SetSlider();
    }

    private void FixedUpdate()
    {
        CheckIsGrounded();
        CheckIsGrabWall();
        if (CheckHP())
        { 
            MoveCharge();
            MoveRelease();
            MoveWalk();
            MoveJump();
        }
        SetState();
    }


    #region Move

    private void SetState()
    {
        ridbody.mass = 5f + (currentGas / (maxActorGas * 1.0f)*4);
        ridbody.transform.localScale = new Vector3(currentGas * deltaScale + 1, currentGas * deltaScale + 1, currentGas * deltaScale + 1);
    }
    private void MoveWalk()
    {
        if (this.GetComponent<CollisionStun>().fall)
            return;
        if (axisInput.magnitude > movementThrashold)
        {
            targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + characterCamera.transform.eulerAngles.y;
            Vector3 forceForward = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector3 rotasionForward = new Vector3(-axisInput.x, 0,axisInput.y);
            ridbody.gameObject.GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Slerp(ridbody.gameObject.GetComponent<ConfigurableJoint>().targetRotation, Quaternion.Euler(0,-targetAngle,0), 0.1f);
            ridbody.AddForce(forceForward * movementSpeed);
            isWalk = true;
        }
        else
        {
            isWalk = false;
        }
    }

    private void MoveJump()
    {
        if (this.GetComponent<CollisionStun>().fall)
            return;
        if (jump && (isGrounded || isGrabWall))
        {
            ridbody.AddForce(Vector3.up * jumpForce,ForceMode.Impulse);
            //isGrounded = false;
        }
    }

    private void MoveCharge()
    {
        if(charge && grab.weapon == null)
        {
            if(currentGas < maxActorGas && !releasing)
            {
                currentGas = currentGas + (maxActorGas - currentGas) / chargeTime * Time.fixedDeltaTime;
                neckPoint.targetRotation = Quaternion.Euler(0, 0, -45);
                releasing = false;
            }
        }

        if (charge && grab.weapon)
        {
            grab.weapon.Fire();
        }

        else if (grab.weapon)
        {
            grab.weapon.StopFire();
        }
    }

    private void MoveRelease()
    {
        if(!charge || releasing)
        {
            if(currentGas > 0)
            {
                var releaseDir = new Vector3(ridbody.transform.right.x, Mathf.Clamp(ridbody.transform.right.y, -0.1f, 0.1f), ridbody.transform.right.z);
                releaseDir = releaseDir.normalized;
                if (currentGas < maxActorGas/20)
                {
                    currentGas = 0;
                    releasing = false;
                    releaseEffect.gameObject.SetActive(false);
                }
                else if(currentGas > 0)
                {
                    if (!releasing)
                    {
                        var addSpeed = (currentGas / (maxActorGas)) * 120f;
                        ridbody.AddForce(releaseDir * releaseSpeed * addSpeed);
                    }
                    else 
                    {
                        ridbody.AddForce(releaseDir * releaseSpeed * 2f);
                    }
                    currentGas = currentGas - (currentGas) / releaseTime * Time.fixedDeltaTime;
                    neckPoint.targetRotation = Quaternion.Euler(0, 0, 0);
                    releaseEffect.gameObject.SetActive(true);
                    releasing = true;
                }
                else
                {
                    currentGas = currentGas - (currentGas) / releaseTime * Time.fixedDeltaTime;
                    neckPoint.targetRotation = Quaternion.Euler(0, 0, 0);
                    ridbody.AddForce(releaseDir * releaseSpeed * 2f);
                }
            }
        }
    }
    private void MoveCamera()
    {
        characterCamera.GetComponent<CameraFollow>().angularDisplacement += cameraInputX;
    }

    #endregion

    #region Check

    private void CheckIsGrounded()
    {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, gameObject.GetComponent<SphereCollider>().radius * transform.localScale.x, 0), 0.05f, groundMask)|| Physics.CheckSphere(transform.position - new Vector3(0, gameObject.GetComponent<SphereCollider>().radius * transform.localScale.x, 0), 0.05f,LayerMask.GetMask("Column"));
    }

    private void CheckIsGrabWall()
    {
        if (grab.grabbedObj)
            isGrabWall = grab.grabbedObj.layer == LayerMask.NameToLayer("Column");
        else
            isGrabWall = false;
    }

    private bool CheckHP()
    {
        if(isSwimmy)
        {
            currentHPValue = currentHPValue + maxHPValue / cureTime * Time.fixedDeltaTime;
            currentHPValue = Mathf.Min(currentHPValue, maxHPValue);
            if(currentHPValue == maxHPValue)
            {
                isSwimmy = false;
            }
            return false;
        }

        if(currentHPValue <= 0)
        {
            GetComponent<CollisionStun>().maxFallTime = cureTime;
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
        canvas.transform.forward = characterCamera.transform.forward;
        hpSlider.transform.position = transform.position;
        gpSlider.transform.position = transform.position;
        hpSlider.transform.localPosition = hpSlider.transform.localPosition + new Vector3(0, 1.4f + (transform.localScale.x - 1) * 1.2f, 0);
        gpSlider.transform.localPosition = gpSlider.transform.localPosition + new Vector3(0, 1f + (transform.localScale.x - 1) * 1.2f, 0);
        otherHpSlider.value = (float)(currentHPValue / maxHPValue);
        otherGpSlider.value = (float)(currentGas / maxActorGas);
        otherCanvas.transform.forward = otherCharacterCamera.transform.forward;
        otherHpSlider.transform.position = transform.position;
        otherGpSlider.transform.position = transform.position;
        otherHpSlider.transform.localPosition = otherHpSlider.transform.localPosition + new Vector3(0, 1.4f + (transform.localScale.x - 1) * 1.2f, 0);
        otherGpSlider.transform.localPosition = otherGpSlider.transform.localPosition + new Vector3(0, 1f + (transform.localScale.x - 1) * 1.2f, 0);
    }
}
