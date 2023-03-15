using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
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
    [SerializeField]public LayerMask groundMask;
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
    private bool pickup;
    private bool useWeapon;
    private bool chargingWeapon;

    //摇杆输入值最小值
    private float movementThrashold = 0.01f;
    [Space(10)]
    [Header("相关需要关联组件")]
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
    public ScaleCollider scaleCollider;
    //public Grab grab;
    public CollisionStun collisionStun;
    public Rigidbody ridbody;
    public ConfigurableJoint cj;
    public Collider bodyCollider;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public float originalRadius;
    public float targetRadius;
    public Vector3 targetCenter;
    public Vector3 originalCenter;

    public Transform weaponPoint;

    [SerializeField]
    public string[] tags;

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


    private float deltaScale;

    //public MeleeWeapon weapon;
    //public MeleeWeapon holdingWeapon;
    //public ConfigurableJoint leftHand;
    //public ConfigurableJoint rightHand;

    public bool swinging = false;
    public bool readyswing = false;

    private void Awake()
    {
        currentHPValue = maxHPValue;
        deltaScale = (maxScale - 1) / maxActorGas;
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
        cameraInputX = inputReader.cameraInput.x;
        pickup = inputReader.pull;
        useWeapon = inputReader.interact;
        MoveCamera();
        //AdjustHand();
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
            //PickUpWeapon();
            //UseWeapon();
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
        //ridbody.mass = 5f + (currentGas / (maxActorGas * 1.0f)*4);
        var gasScale = (currentGas / (maxActorGas * 1.0f));
        // skinnedMeshRenderer.SetBlendShapeWeight(0, gasScale * 100f);
        if(scaleCollider)
            scaleCollider.SetScale(gasScale);
        //(bodyCollider as SphereCollider).center = Vector3.Lerp(originalCenter, targetCenter, gasScale);
        (bodyCollider as SphereCollider).radius = Mathf.Lerp(originalRadius, targetRadius, gasScale);
        //ridbody.transform.localScale = new Vector3(currentGas * deltaScale + 1, currentGas * deltaScale + 1, currentGas * deltaScale + 1);
    }
    private void MoveWalk()
    {
        if (collisionStun.fall)
            return;
        if (axisInput.magnitude > movementThrashold)
        {
            targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + characterCamera.transform.eulerAngles.y;
            Vector3 forceForward = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector3 rotasionForward = new Vector3(-axisInput.x, 0,axisInput.y);
            //ridbody.gameObject.GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Slerp(ridbody.gameObject.GetComponent<ConfigurableJoint>().targetRotation, Quaternion.Euler(0,-targetAngle,0), 0.1f);
            //ridbody.gameObject.GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Euler(0,-targetAngle,0);
            if (isGrounded)
                ridbody.velocity =new Vector3(forceForward.x * movementSpeed ,ridbody.velocity.y ,forceForward.z * movementSpeed);
                //ridbody.AddForce(forceForward * movementSpeed);
            isWalk = true;
        }
        else
        {
            isWalk = false;
        }
        ridbody.gameObject.GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Slerp(ridbody.gameObject.GetComponent<ConfigurableJoint>().targetRotation , Quaternion.Euler(0,-targetAngle,0), 0.1f);
    }

    //private void UseWeapon()
    //{
    //    if (holdingWeapon && useWeapon)
    //    {
    //        holdingWeapon.OnCharge();
    //        chargingWeapon = true;
    //    }
    //    else if (!useWeapon && chargingWeapon)
    //    {
    //        holdingWeapon.OnRelease();
    //        chargingWeapon = false;
    //        //DropWeapon();
    //    }
    //}

    //private void PickUpWeapon()
    //{
    //    if (collisionStun.fall)
    //    {
    //        DropWeapon();
    //    }
    //    else
    //    {
    //        if (pickup)
    //        {
    //            if (holdingWeapon == null && weapon != null && weapon.controller == null)
    //            {
    //                holdingWeapon = weapon;

    //                weapon.transform.parent = this.weaponPoint;
    //                weapon.transform.localPosition = weapon.offsetPosition;
    //                weapon.transform.localRotation = Quaternion.Euler(weapon.offsetRotation);

    //                leftHand.targetRotation = Quaternion.Euler(holdingWeapon.targetRotationL);
    //                rightHand.targetRotation = Quaternion.Euler(holdingWeapon.targetRotationR);

    //                weapon.OnEquipped(this);

    //            }
    //        }
    //    }
    //}

    //public void DropWeapon()
    //{
    //    if (holdingWeapon != null)
    //    {
    //        holdingWeapon.OnUnEquipped();
    //        holdingWeapon = null;
    //    }
    //}

    private void MoveJump()
    {
        if (collisionStun.fall)
            return;
        if (jump && (isGrounded || isGrabWall))
        {
            ridbody.AddForce(Vector3.up * jumpForce,ForceMode.Impulse);
            //isGrounded = false;
        }
    }

    private void MoveCharge()
    {
        if (collisionStun.fall)
            return;
        if (charge)
        {
            if(currentGas < maxActorGas && !releasing)
            {
                currentGas = currentGas + (maxActorGas - currentGas) / chargeTime * Time.fixedDeltaTime;
                releasing = false;
            }
        }

        //if (charge && grab.weapon)
        //{
        //    grab.weapon.Fire();
        //}

        //else if (grab.weapon)
        //{
        //    grab.weapon.StopFire();
        //}
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
            if(currentGas > 0)
            {
                var releaseDir = new Vector3(-ridbody.transform.forward.x, 0, -ridbody.transform.forward.z);
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
                        var addSpeed = EaseOutCirc(currentGas / (maxActorGas)) * 100f;
                        ridbody.AddForce(releaseDir * releaseSpeed * addSpeed);
                    }
                    else 
                    {
                        var addSpeed = EaseOutCirc(currentGas / (maxActorGas)) * 2;
                        ridbody.AddForce(releaseDir * releaseSpeed * addSpeed);
                    }
                    currentGas = currentGas - (currentGas) / releaseTime * Time.fixedDeltaTime;
                    releaseEffect.gameObject.SetActive(true);
                    releasing = true;
                }
                else
                {
                    var addSpeed = EaseOutCirc(currentGas / (maxActorGas)) * 2;
                    currentGas = currentGas - (currentGas) / releaseTime * Time.fixedDeltaTime;
                    ridbody.AddForce(releaseDir * releaseSpeed * addSpeed);
                }
            }
        }
    }

    //private void AdjustHand()
    //{
    //    if (leftHand == null || rightHand == null)
    //        return;
    //    if (holdingWeapon)
    //    {
    //        leftHand.targetRotation = Quaternion.Euler(Vector3.Lerp(leftHand.targetRotation.eulerAngles, holdingWeapon.targetRotationL, 0.1f));
    //        rightHand.targetRotation = Quaternion.Euler(Vector3.Lerp(rightHand.targetRotation.eulerAngles, holdingWeapon.targetRotationR, 0.1f));
    //    }
    //    else
    //    {
    //        leftHand.targetRotation = Quaternion.Euler(Vector3.Lerp(leftHand.targetRotation.eulerAngles, Vector3.zero, 0.1f));
    //        rightHand.targetRotation = Quaternion.Euler(Vector3.Lerp(rightHand.targetRotation.eulerAngles, Vector3.zero, 0.1f));
    //    }
    //}

    private void MoveCamera()
    {
        characterCamera.GetComponent<CameraFollow>().angularDisplacement += cameraInputX;
    }

    #endregion

    #region Check

    private void CheckIsGrounded()
    {
        isGrounded = Physics.CheckSphere(bodyCollider.transform.position - new Vector3(0, (bodyCollider as SphereCollider).radius / 2, 0), 0.5f, groundMask);
    }

    private void CheckIsGrabWall()
    {
        //if (grab && grab.grabbedObj)
        //    isGrabWall = grab.grabbedObj.layer == LayerMask.NameToLayer("Column");
        //else
            isGrabWall = false;
    }

    private bool CheckHP()
    {

        if (currentHPValue == lastHPValue)
            HPtimer += Time.fixedDeltaTime;
        else
            HPtimer = 0;

        if(HPtimer > 15)
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
            if(currentHPValue == maxHPValue)
            {
                isSwimmy = false;
            }
            return false;
        }

        if(currentHPValue <= 0)
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
        canvas.transform.forward = characterCamera.transform.forward;
        hpSlider.transform.position = bodyCollider.transform.position;
        gpSlider.transform.position = bodyCollider.transform.position;
        hpSlider.transform.localPosition = hpSlider.transform.localPosition + new Vector3(0, 2.4f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
        gpSlider.transform.localPosition = gpSlider.transform.localPosition + new Vector3(0, 2f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
        otherHpSlider.value = (float)(currentHPValue / maxHPValue);
        otherGpSlider.value = (float)(currentGas / maxActorGas);
        otherCanvas.transform.forward = otherCharacterCamera.transform.forward;
        otherHpSlider.transform.position = bodyCollider.transform.position;
        otherGpSlider.transform.position = bodyCollider.transform.position;
        otherHpSlider.transform.localPosition = otherHpSlider.transform.localPosition + new Vector3(0, 2.4f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
        otherGpSlider.transform.localPosition = otherGpSlider.transform.localPosition + new Vector3(0, 2f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    foreach (var item in tags)
    //    {
    //        if (other.gameObject.CompareTag(item))
    //        {
    //            if (other.gameObject.GetComponent<MeleeWeapon>())
    //                weapon = other.gameObject.GetComponent<MeleeWeapon>();
    //        }
    //    }
    //}


    //private void OnTriggerExit(Collider other)
    //{
    //    weapon = null;
    //}
}
