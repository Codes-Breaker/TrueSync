using System;
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
    public Image drownImage;
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

    public float drowningSpeed = 1;
    public float maxDrowning = 1000;
    public float currentDrown = 0;

    public float invulernableTime = 0;
    public float maxInvulnerableTime = 5;
    //是否处于无敌
    public bool invulernable = false;
    //碰撞受击累积值
    public int vulnerbility = 0;
    //是否正在返回
    public bool returning = false;
    //是否正在跳跃
    public bool jumpingBack = false;
    //游泳目的地
    public Vector3 swimTarget = Vector3.zero;
    //跳跃目的地
    public Vector3 jumpTarget = Vector3.zero;
    //游泳速度
    public float swimSpeed = 1;

    private bool isDead = false;

    private int defaultLayer = 0;

    private void Awake()
    {
        speedUpGas = maxSpeedUpGas;
        currentHPValue = maxHPValue;
        isGrounded = true;
        originalRadius = (bodyCollider as SphereCollider).radius;
        originalCenter = (bodyCollider as SphereCollider).center;
        defaultLayer = this.gameObject.layer;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.ridbody.position, swimTarget);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(this.ridbody.position, jumpTarget);
    }


    private void FixedUpdate()
    {
        velocityBeforeCollision = GetComponent<Rigidbody>().velocity;
        positionBeforeCollision = GetComponent<Rigidbody>().position;
        CheckInVulernable();
        CheckIsGrounded();
        if (!isDead)
        {
            if (returning)
            {
                ReturnToPlace();
            }
            else if (jumpingBack)
            {
                JumpToPlace();
            }
            else
            {
                MoveCharge();
                MoveRelease();
                MoveWalk();
                MoveJump();
            }

        }
        SetState();
        if (!isGrounded)
        {
            //SetGravity();
        }
    }

    private void CheckInVulernable()
    {
        if (invulernable && invulernableTime > 0)
        {
            invulernableTime -= Time.fixedDeltaTime;
            if (invulernableTime <= 0)
            {
                invulernable = false;
                SetCollider(true);
                SetFlashMeshRendererBlock(false);
            }
        }
    }

    private void SetGameLayerRecursive(GameObject _go, int _layer)
    {
        _go.layer = _layer;
        foreach (Transform child in _go.transform)
        {
            child.gameObject.layer = _layer;

            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
                SetGameLayerRecursive(child.gameObject, _layer);

        }
    }

    private void SetCollider(bool set)
    {
        if (set)
        {
            this.gameObject.layer = defaultLayer;
            //SetGameLayerRecursive(this.gameObject, defaultLayer);
            foreach (Transform child in transform)
            {
                SetGameLayerRecursive(child.gameObject, 17);
            }
        }
        else
        {
            this.gameObject.layer = 17;
            //SetGameLayerRecursive(this.gameObject, 17);
            foreach (Transform child in transform)
            {
                SetGameLayerRecursive(child.gameObject, 17);
            }
        }

    }

    private void SetKinematics(bool set)
    {
        var childrens = this.transform.GetComponentsInChildren<Rigidbody>();
        foreach(var child in childrens)
        {
            child.isKinematic = set;
            child.gameObject.GetComponent<Collider>().enabled = !set;
        }
        this.ridbody.isKinematic = set;
        this.ridbody.gameObject.GetComponent<Collider>().enabled = !set;
    }


    public void SetUpJump()
    {
        startPos = ridbody.transform.position;
        HDist = (jumpTarget - new Vector3(ridbody.transform.position.x, jumpTarget.y, ridbody.transform.position.z)).magnitude;
        HDist *= 1.2f;
        curTime = 0;
        currentGas = 0;
        vulnerbility = 0;
        releasing = false;
        drowningSpeed++;
        SetKinematics(true);
    }

    private float HDist = 0;
    private float curTime = 0;
    private float maxTime = 1f;
    private Vector3 startPos;
    private float maxHeight = 5;
    private void JumpToPlace()
    {
        if (!jumpingBack)
            return;
        curTime += Time.fixedDeltaTime;
        var hDelta = jumpTarget - startPos;
        hDelta.y = 0;
        var hPos = Mathf.Lerp(0, HDist, curTime/ maxTime) * hDelta.normalized;
        var v = Mathf.Lerp(0, maxHeight, Mathf.Sin(Mathf.Lerp(0, (3f / 4f) * Mathf.PI, curTime / maxTime)));
        
        Vector3 currentPos = startPos + new Vector3(hPos.x, v, hPos.z);
        ridbody.transform.position = currentPos;
        if (curTime >= maxTime)
        {
            jumpingBack = false;
            SetKinematics(false);
            SetCollider(false);
            invulernable = true;
            invulernableTime = maxInvulnerableTime;
            SetFlashMeshRendererBlock(true);
        }
        currentDrown = 0;
    }

    // 无敌特效示意
    private void SetFlashMeshRendererBlock(bool value)
    {
        var rendererBlock = new MaterialPropertyBlock();
        skinnedMeshRenderer.GetPropertyBlock(rendererBlock, 0);
        rendererBlock.SetFloat("_PlayHurt", value ? 1f : 0f);
        skinnedMeshRenderer.SetPropertyBlock(rendererBlock, 0);

        var rendererBlock1 = new MaterialPropertyBlock();
        skinnedMeshRenderer.GetPropertyBlock(rendererBlock1, 1);
        rendererBlock1.SetFloat("_PlayHurt", value ? 1f : 0f);
        skinnedMeshRenderer.SetPropertyBlock(rendererBlock1, 1);

        var rendererBlock2 = new MaterialPropertyBlock();
        skinnedMeshRenderer.GetPropertyBlock(rendererBlock2, 2);
        rendererBlock2.SetFloat("_PlayHurt", value ? 1f : 0f);
        skinnedMeshRenderer.SetPropertyBlock(rendererBlock2, 2);
    }

    private void ReturnToPlace()
    {
        targetAngle = Mathf.Atan2(swimTarget.x, swimTarget.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        this.ridbody.transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), 0.1f);
        var dir = (swimTarget - ridbody.transform.position).normalized;
        ridbody.transform.position = ridbody.transform.position + (dir * swimSpeed * Time.fixedDeltaTime);
        AccumulateDrown();
    }

    private void AccumulateDrown()
    {
        currentDrown += drowningSpeed;
        if (currentDrown >= maxDrowning)
        {
            isDead = true;
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
            hpSlider.value = (float)(vulnerbility / maxHPValue);
            gpSlider.value = (float)(currentGas / maxActorGas);
            canvas.transform.forward = Camera.main.transform.forward;
            hpSlider.transform.position = bodyCollider.transform.position;
            gpSlider.transform.position = bodyCollider.transform.position;
            hpSlider.transform.localPosition = hpSlider.transform.localPosition + new Vector3(0, 1.6f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
            gpSlider.transform.localPosition = gpSlider.transform.localPosition + new Vector3(0, 1.3f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
            drownImage.transform.position = bodyCollider.transform.position;
            drownImage.transform.localPosition = drownImage.transform.localPosition + new Vector3(-1, 1.5f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
            drownImage.fillAmount = currentDrown / maxDrowning;
        }

    }

    private void OnCollisionStay(Collision collision)
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

            ridbody.AddExplosionForce((froceArgument + vulnerbility) * m2 * (0.2f), collision.contacts[0].point, 4);
            collision.collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce((froceArgument + otherCollision.vulnerbility) * m2 * (0.2f), collision.contacts[0].point, 4);
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

            vulnerbility += Convert.ToInt32(m2 * 2);

            ridbody.AddExplosionForce((froceArgument + vulnerbility) * m2, collision.contacts[0].point, 4);
            collision.collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce((froceArgument + otherCollision.vulnerbility)* m2, collision.contacts[0].point, 4);
            
        }
    }

}
