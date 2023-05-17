using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using Cinemachine;
using Crest;
using RangeAttribute = UnityEngine.RangeAttribute;
using RootMotion.FinalIK;

public class CharacterContorl : MonoBehaviour
{
    [Header("控制器")]
    public InputReaderBase inputReader;
    [Space(10)]
    [Header("走路参数")]
    [Header("走路速度提升需要时间")]
    public float movementSpeedUpTime;
    [Header("走路转向Lerp系数")]
    public float movementRotationRate;
    [Header("最大行走速度")]
    public float movementMaxVelocity;
    [Space(10)]
    [Header("跳跃力系数")]
    public float jumpForce;
    [Space(10)]
    [Header("跑步状态相关参数")]
    [Header("跑步最大速度")]
    public float runMaxVelocity;
    [Header("跑步速度提升需要时间")]
    public float runSpeedUpTime;
    [Header("跑步速度提升时转向Lerp系数")]
    public float runSpeedUpRotationRate;
    [Header("跑步最大速度时转向Lerp系数")]
    public float runMaxSpeedRotationRate;
    [Header("跑步充能时间")]
    public float chargeTime;
    [Header("最大跑步时间")]
    public float releaseTime;
    [Space(10)]
    [Header("刹车相关参数")]
    [Header("减速力系数系数")]
    public float decelerationForceArgument;
    [Header("减速扭矩力系数系数")]
    public float decelerationTorqueArgument;

    [Space(10)]
    [Header("斜面相关设置")]
    [Header("地面的Layers")]
    [SerializeField] public LayerMask groundMask;
    [Header("斜坡距离底部的检测距离")]
    public float slopeCheckerThrashold = 0.51f;
    [Header("斜坡检测点的向前偏移")]
    public float slopeCheckerForwardOffset = 0.41f;
    [Header("最小斜坡角度")]
    public float miniClimbableSlopeAngle = 5;
    [Header("最大斜坡角度")]
    public float maxClimbableSlopeAngle = 40f;
    
    [Space(10)]
    [Header("眩晕槽相关设置")]
    [Header("眩晕槽最大值")]
    private float maxHPValue = 100;
    [Header("撞击力映射打击数值的曲线")]
    public AnimationCurve forceToHuntCurve;
    [Header("打击恢复时间")]
    public float cureTime;

   
    private bool hasJump = false;
    private bool hasBrake = false;


    //摇杆输入值最小值
    private float movementThrashold = 0.35f;
    //public float originalRadius;
    //public float targetRadius;
    //public Vector3 targetCenter;
    //public Vector3 originalCenter;
    public float sensitivity = 0.5f;
    private Vector3 initialRotation;
    private Vector3 forceTarget;
    private Quaternion initialRot;
    // 隐藏参数
    private bool isWalk;
    private bool isGrounded;
    [HideInInspector]
    public float targetAngle;
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
    public float HPtimer;
    [HideInInspector]
    public SkillItemControllerBase skill;
    [HideInInspector]
    public bool isUseSkill;
    //攻击力
    public float forceArgument;
    //防御力系数
    [Range(0, 2)]
    public float receivedForceRate = 1;

    //public bool swinging = false;
    //public bool readyswing = false;

    int speedUpGas = 0;
    public int maxSpeedUpGas = 5;

    [HideInInspector]
    public Vector3 velocityBeforeCollision = Vector3.zero;
    [HideInInspector]
    public Vector3 positionBeforeCollision = Vector3.zero;

    public float maxDrowning = 1000;
    private float maxDrownValue = 1000;
    public float currentDrown = 0;
    private float totalDrown = 0;
    private const int minDrown = 50;

    [Range(0, 1)]
    public float continueReceivedForceRate = 0.2f;

    public float invulernableTime = 0;
    public bool invulernable = false;
    private Vector3 groundNormal;
    private bool isTouchingSlope = false;

    [Range(0, 1)]
    //易伤系数
    public float vulerbilityReactRate = 1;
    ////是否正在返回
    //public bool returning = false;
    ////是否正在跳跃
    //public bool jumpingBack = false;
    ////游泳目的地
    //public Vector3 swimTarget = Vector3.zero;
    ////跳跃目的地
    //public Vector3 jumpTarget = Vector3.zero;
    ////游泳速度
    //public float swimSpeed = 1;

    public bool isDead = false;

    private int defaultLayer = 0;

    //眩晕槽
    public float stunTime = 0;
    //眩晕时间
    public float maxStunTime = 0;
    //是否眩晕
    private bool isStun = false;
    [Space(10)]
    [Header("相关需要关联组件")]
    public Slider gpSlider;
    public Slider hpSlider;
    public Image drownImage;
    public Canvas canvas;
    public Rigidbody ridbody;
    public Collider bodyCollider;
    //public SkinnedMeshRenderer skinnedMeshRenderer;
    public MeshRenderer ringRenderer;
    public Animator anima;
    //public MaterialController meshController;
    //特效资源
    public GameObject stunEffect;
    //眩晕阈值
    public float stunThreshold;

    public List<Buff> buffs = new List<Buff>();

    private GameController gameController;

    public int playerIndex = -1;

    public SimpleFloatingObject floatObj;
    public GrounderQuadruped grounderQuadruped;

    public bool isInWater = false;
    [Header("撞击速度攻击曲线")]
    public AnimationCurve attackForceFeedback;
    [Header("受击速度攻击曲线")]
    public AnimationCurve hitForceFeedback;

    private void Awake()
    {
        speedUpGas = maxSpeedUpGas;
        currentHPValue = maxHPValue;
        isGrounded = true;
        //originalRadius = (bodyCollider as SphereCollider).radius;
        //originalCenter = (bodyCollider as SphereCollider).center;
        defaultLayer = this.gameObject.layer;
        initialRotation = ridbody.transform.rotation.eulerAngles;
        initialRot = ridbody.transform.rotation;
        currentDrown = maxDrowning;
        gameController = GameObject.Find("GameManager").GetComponent<GameController>();
       // SetFlashMeshRendererBlock(false);
        maxDrownValue = maxDrowning;
    }

    private void Start()
    {
        SetControlSelf();
        SetRingColor();
    }

    private void LateUpdate()
    {
        SetSlider();
    }

    private void Update()
    {
        SetAnimatorArgument();
        SetIK();
    }

    private void FixedUpdate()
    {
        velocityBeforeCollision = GetComponent<Rigidbody>().velocity;
        positionBeforeCollision = GetComponent<Rigidbody>().position;
        CheckInVulernable();
        CheckIsGrounded();
        UpdateBuff();
        CheckSlopeAndDirections();

        CheckIsInWater();
        if (!isGrounded)
        {
            //SetGravity();
        }
    }

    public void SetControlSelf()
    {
        inputReader.moveAciotn = MoveWalk;
        inputReader.chargeAction = MoveCharge;
        inputReader.releaseAciton = MoveRelease;
        inputReader.interactWeaponAction = UseWeapon;
        inputReader.jumpAction = MoveJump;
        inputReader.brakeAciton = MoveBrake;
    }

    private void SetStun()
    {
        stunTime += Time.fixedDeltaTime;
        if (stunTime >= maxStunTime)
        {
            isStun = false;
            stunTime = 0;
            stunEffect.gameObject.SetActive(false);
        }
    }

    private void Stun(float time)
    {
        maxStunTime = time;
        isStun = true;
        stunEffect.gameObject.SetActive(true);
    }

    private void UpdateBuff()
    {
        foreach(var buff in buffs.ToArray())
        {
            if (!buff.isEnd)
                buff.OnBuffUpdate();
            if (buff.isEnd)
            {
                buffs.Remove(buff);
            }
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
               // SetFlashMeshRendererBlock(false);
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

    private void SetAnimatorArgument()
    {
        var speed = new Vector3(ridbody.velocity.x, 0, ridbody.velocity.z).magnitude;
        anima.SetFloat("Speed", speed);
    }

    private void SetCollider(bool set)
    {
        if (set)
        {
            this.gameObject.layer = defaultLayer;
            //SetGameLayerRecursive(this.gameObject, defaultLayer);
            foreach (Transform child in transform)
            {
                if (child.gameObject.layer != 18)
                    SetGameLayerRecursive(child.gameObject, 17);
            }
        }
        else
        {
            this.gameObject.layer = 17;
            //SetGameLayerRecursive(this.gameObject, 17);
            foreach (Transform child in transform)
            {
                if (child.gameObject.layer != 18)
                    SetGameLayerRecursive(child.gameObject, 17);
            }
        }

    }

    public void SetKinematics(bool set)
    {
        //var childrens = this.transform.GetComponentsInChildren<Rigidbody>();
        //foreach(var child in childrens)
        //{
        //    child.isKinematic = set;
        //    child.gameObject.GetComponent<Collider>().enabled = !set;
        //}
        this.ridbody.isKinematic = set;
        this.ridbody.gameObject.GetComponent<Collider>().enabled = !set;
    }

    public void SetUpReturn()
    {
        currentDrown = maxDrowning;
        //SetFlashMeshRendererBlock(false);
        foreach (var buff in buffs)
        {
            buff.Finish();
        }
    }

    public void GetHurt()
    {

    }


    private void AccumulateDrown()
    {
        currentDrown -= 1;
        totalDrown += 1;

        if (currentDrown <= 0)
        {
            isDead = true;
            Dead();
        }
    }

    private void Dead()
    {
        var cinemachineTargetGroup = GameObject.FindObjectOfType<CinemachineTargetGroup>();
        cinemachineTargetGroup.RemoveMember(transform);
        this.gameObject.SetActive(false);
        this.canvas.gameObject.SetActive(false);
        gameController.CheckGameState();
    }

    private void SetGravity()
    {
        ridbody.AddForce(Physics.gravity - Vector3.one, ForceMode.Acceleration);
    }


    #region Move

    private bool hasLglooStun()
    {
        foreach (var buff in buffs)
        {
            if (buff is LglooBuff)
            {
                return true;
            }
        }
        return false;
    }

    private bool isBuffStun()
    {
        foreach(var buff in buffs)
        {
            if (buff is StunBuff)
            {
                return true;
            }
        }
        return false;
    }

    private void MoveWalk(Vector2 axisInput,ControlDeviceType controlDeviceType)
    {
        //单位化输入方向
        if (controlDeviceType == ControlDeviceType.Mouse)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            var detalPosition = axisInput - new Vector2(screenPosition.x, screenPosition.y);
            axisInput = detalPosition.normalized;
        }
        else
            axisInput = axisInput.normalized;
        //
        if(releasing)
        {
            isWalk = false;
            if(ridbody.velocity.magnitude < runMaxVelocity * 0.96f)
            {
                var acceleration = runMaxVelocity / runSpeedUpTime;
                var forceMagnitude = ridbody.mass * acceleration;
                var gravityDivide = Vector3.zero; 
                //if (bodyCollider.material && (isGrounded||isTouchingSlope))
                //{
                    if(isTouchingSlope||isGrounded)
                    {
                        gravityDivide = Vector3.ProjectOnPlane(Physics.gravity, groundNormal) * ridbody.mass;
                        var gravityFrictionDivide = Physics.gravity - gravityDivide;
                        var frictionForceMagnitude = ridbody.mass * bodyCollider.material.dynamicFriction * gravityFrictionDivide.magnitude;
                        forceMagnitude = forceMagnitude + frictionForceMagnitude;
                    }
                
                //补偿重力分量
                var moveTarget = ridbody.transform.forward;
                moveTarget = moveTarget.normalized;
                moveTarget = Vector3.ProjectOnPlane(moveTarget, groundNormal).normalized;
                ridbody.AddForce(moveTarget * forceMagnitude - gravityDivide, ForceMode.Force);
                

                if (axisInput.magnitude > movementThrashold)
                {
                    targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                    transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), runSpeedUpRotationRate);
                }
            }
            else
            {
                if (axisInput.magnitude > movementThrashold)
                {
                    targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                    transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), runMaxSpeedRotationRate);
                }
            }
        }
        else
        {

            if(ridbody.velocity.magnitude < movementMaxVelocity &&axisInput.magnitude > movementThrashold )
            {
                isWalk = true;
                targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                var acceleration = movementMaxVelocity / movementSpeedUpTime;
                var forceMagnitude = ridbody.mass * acceleration;

                var gravityDivide = Vector3.zero;
                //if (bodyCollider.material && (isGrounded || isTouchingSlope))
                //{
                    if (isTouchingSlope || isGrounded)
                    {
                        gravityDivide = Vector3.ProjectOnPlane(Physics.gravity, groundNormal) * ridbody.mass;
                        var gravityFrictionDivide = Physics.gravity - gravityDivide;
                        var frictionForceMagnitude = ridbody.mass * bodyCollider.material.dynamicFriction * gravityFrictionDivide.magnitude;
                        forceMagnitude = forceMagnitude + frictionForceMagnitude;
                    }
                
                var moveTarget = ridbody.transform.forward;
                moveTarget = moveTarget.normalized;
                moveTarget = Vector3.ProjectOnPlane(moveTarget, groundNormal).normalized;
                ridbody.AddForce(moveTarget * forceMagnitude - gravityDivide, ForceMode.Force);
                Debug.Log(forceMagnitude);
                transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), movementRotationRate);
            }
            else
            {
                isWalk = false;
            }


        }
        
    }

   
    private void MoveBrake(bool brake)
    {
        if (brake)
        {
            ridbody.AddForce(-ridbody.velocity * decelerationForceArgument);
            ridbody.AddTorque(-ridbody.angularVelocity * decelerationTorqueArgument);
            if(!hasBrake)
            {
                anima.SetBool("isBrake", true);
                hasBrake = true;
            }
        }
        else
        {
            if(hasBrake)
            {
                anima.SetBool("isBrake", false);
                hasBrake = false;
            }
        }
    }

    private void MoveJump(bool jump)
    {
        if (jump && (isGrounded||isTouchingSlope||isInWater) && !hasJump)
        {
            ridbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            hasJump = true;

        }

        if (!jump && (isGrounded || isTouchingSlope || isInWater) && hasJump)
            hasJump = false;
    }

    private void UseWeapon(bool isUse)
    {
        if(isUse && skill && currentGas == 0)
        {
            skill.UseSkillItem();
        }
    }

    private void MoveCharge(bool charge)
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
            anima.SetBool("Charge", true);
        }
        else
        {
            anima.SetBool("Charge", false);
        }
    }

    private float EaseOutCirc(float number)
    {
        return Mathf.Sqrt(1 - Mathf.Pow(number - 1, 2));
    }


    private float releaseGasGauge = 0;
    private void MoveRelease(bool charge)
    {
        if (!charge || releasing)
        {
            if (currentGas == 0)
            {
                currentGas = 0;
                releasing = false;
                speedUpGas = maxSpeedUpGas;
            }
            else if (currentGas > 0)
            {
                currentGas = currentGas - (maxActorGas) / releaseTime * Time.fixedDeltaTime;
                currentGas = Mathf.Max(0, currentGas);


                releasing = true;
                
            }
        }
    }

    #endregion

    #region Check
    private void SetIK()
    {
        var targetIK = isInWater ? 0 : 1;

        if (grounderQuadruped.weight != targetIK)
        {
            var speed = Time.deltaTime;
            if (targetIK > grounderQuadruped.weight)
            {
                grounderQuadruped.weight += speed;
                grounderQuadruped.weight = Mathf.Min(1, grounderQuadruped.weight);
            }
            else
            {
                grounderQuadruped.weight -= speed;
                grounderQuadruped.weight = Mathf.Max(0, grounderQuadruped.weight);
            }
        }
    }

    private void CheckIsInWater()
    {
        isInWater = floatObj.InWater;
    }

    private void CheckIsGrounded()
    {
        isGrounded = Physics.CheckSphere(bodyCollider.transform.position+(bodyCollider as SphereCollider).center - new Vector3(0, (bodyCollider as SphereCollider).radius , 0), 0.02f, groundMask);

    }

    private void CheckSlopeAndDirections()
    {
        RaycastHit slopeHit;
        isTouchingSlope = false;
        if(Physics.SphereCast(bodyCollider.transform.position + (bodyCollider as SphereCollider).center + transform.forward.normalized * slopeCheckerForwardOffset, slopeCheckerThrashold,Vector3.down,out slopeHit, (bodyCollider as SphereCollider).radius + 0.2f, groundMask))
        {
            groundNormal = slopeHit.normal;

            if(Vector3.Angle(Vector3.up, slopeHit.normal) > miniClimbableSlopeAngle && Vector3.Angle(Vector3.up, slopeHit.normal) < maxClimbableSlopeAngle)
            {
                if(!isGrounded)
                    isTouchingSlope = true;
            }
        }
        else
        {
            
        }
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


        //if (isSwimmy)
        //{
        //    currentHPValue = currentHPValue + maxHPValue / cureTime * Time.fixedDeltaTime;
        //    currentHPValue = Mathf.Min(currentHPValue, maxHPValue);
        //    if (currentHPValue == maxHPValue)
        //    {
        //        isSwimmy = false;
        //    }
        //    return false;
        //}

        if (currentHPValue <= 0)
        {
            currentHPValue = 0;
          //  isSwimmy = true;
            return false;
        }
        return true;
    }

    #endregion
    #region SetUI
    private void SetSlider()
    {
        gpSlider.value = (float)(currentGas / maxActorGas);
        hpSlider.value = (float)(maxDrowning / maxDrownValue);
        canvas.transform.forward = Camera.main.transform.forward;
        gpSlider.transform.position = bodyCollider.transform.position;
        hpSlider.transform.position = bodyCollider.transform.position;
        gpSlider.transform.localPosition = gpSlider.transform.localPosition + new Vector3(0, 1.7f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
        hpSlider.transform.localPosition = hpSlider.transform.localPosition + new Vector3(0, 1.9f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
        drownImage.transform.position = bodyCollider.transform.position;
        drownImage.transform.localPosition = drownImage.transform.localPosition + new Vector3(-1, 1.5f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
        drownImage.fillAmount = (maxDrowning - currentDrown) / maxDrowning;
        

    }
    private void SetRingColor()
    {
        var rendererBlock = new MaterialPropertyBlock();
        ringRenderer.GetPropertyBlock(rendererBlock, 0);
        rendererBlock.SetColor("_Color", InputReadManager.Instance.playerColors[playerIndex]);
        ringRenderer.SetPropertyBlock(rendererBlock, 0);
    }
    #endregion

    #region OnCollison
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<TimeLapseBombSkill>())
        {
            //推炸弹
            var eventObjectPrefab = Resources.Load<GameObject>("MediumHit");
            var eventObjectGameObject = Instantiate(eventObjectPrefab, collision.contacts[0].point, Quaternion.Euler(new Vector3(0, 0, 0)));

            var otherCollision = collision.gameObject.GetComponent<TimeLapseBombSkill>();

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

            ridbody.AddExplosionForce(m2 * 2.5f + 300, collision.contacts[0].point, 4);
            collision.collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce((forceArgument + m1)*1.5f + 50, collision.contacts[0].point, 4);
        }
        if (collision.gameObject.GetComponent<CharacterContorl>())
        {
            var otherCollision = collision.gameObject.GetComponent<CharacterContorl>();

            Vector3 vel1 = velocityBeforeCollision;
            Vector3 vel2 = otherCollision.velocityBeforeCollision;

            Vector3 cPoint = collision.contacts[0].point;
            Vector3 contactToMe = cPoint - positionBeforeCollision;
            Vector3 contactToOther = cPoint - otherCollision.positionBeforeCollision;

            var d1 = Vector3.Angle(vel1, contactToMe);
            var d2 = Vector3.Angle(vel2, contactToOther);

            var degree1 = d1 * Mathf.Deg2Rad;
            var degree2 = d2 * Mathf.Deg2Rad;

            Vector3 impactVelocity = collision.relativeVelocity;

            var m1 = (Mathf.Cos(degree1) * vel1).magnitude; //我针对对方的力
            var m2 = (Mathf.Cos(degree2) * vel2).magnitude; //对方针对我的力

            var m = m1 + m2;

            var lglooNerfRate = 1f;
            if (hasLglooStun())
            {
                lglooNerfRate = 0.5f;
            }

            //var attackForce = attackForceFeedback.Evaluate(m2);
            //var hitForce = hitForceFeedback.Evaluate(m1);
            //ridbody.AddExplosionForce((attackForce * bodyCollider.material.staticFriction * ridbody.mass + hitForce * bodyCollider.material.staticFriction * ridbody.mass) * 0.1f, collision.contacts[0].point, 2, 0f, ForceMode.Force);

            //Debug.LogError($"结算m1 : {m1} m2 : {m2} attackForce {attackForce} hitForce {hitForce}");
            //ridbody.AddExplosionForce((otherCollision.forceArgument + m2) * continueReceivedForceRate + 200 * lglooNerfRate, collision.contacts[0].point, 4);
            //collision.collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce((forceArgument + m1) * otherCollision.continueReceivedForceRate + 50, collision.contacts[0].point, 4);

            //ridbody.AddExplosionForce((m1 + m2) * 300f, collision.contacts[0].point, 2, 0f, ForceMode.Force);
            //collision.collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce((m1 + m2 * 0.25f) * 0.1f, collision.contacts[0].point, 2, 0f, ForceMode.VelocityChange);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach(var buff in buffs)
        {
            buff.OnCollide(collision);
        }

        if (collision.gameObject.GetComponent<TimeLapseBombSkill>())
        {
            //推炸弹
            var eventObjectPrefab = Resources.Load<GameObject>("MediumHit");
            var eventObjectGameObject = Instantiate(eventObjectPrefab, collision.contacts[0].point, Quaternion.Euler(new Vector3(0, 0, 0)));

            var otherCollision = collision.gameObject.GetComponent<TimeLapseBombSkill>();

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

            ridbody.AddExplosionForce(m2 * 2.5f + 600, collision.contacts[0].point, 4);
            collision.collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce((forceArgument + m1)*2f + 250, collision.contacts[0].point, 4);
        }

        if (collision.gameObject.GetComponent<CharacterContorl>())
        {
            //特效
            var eventObjectPrefab = Resources.Load<GameObject>("MediumHit");
            var eventObjectGameObject = Instantiate(eventObjectPrefab, collision.contacts[0].point, Quaternion.Euler(new Vector3(0, 0, 0)));

            var otherCollision = collision.gameObject.GetComponent<CharacterContorl>();

            Vector3 vel1 = velocityBeforeCollision;
            Vector3 vel2 = otherCollision.velocityBeforeCollision;

            Vector3 cPoint = collision.contacts[0].point;
            Vector3 contactToMe = cPoint - positionBeforeCollision;
            Vector3 contactToOther = cPoint - otherCollision.positionBeforeCollision;

            var d1 = Vector3.Angle(vel1, contactToMe);
            var d2 = Vector3.Angle(vel2, contactToOther);

            var degree1 = d1 * Mathf.Deg2Rad;
            var degree2 = d2 * Mathf.Deg2Rad;

            Vector3 impactVelocity = collision.relativeVelocity;

            var m1 = (Mathf.Cos(degree1) * vel1).magnitude;
            var m2 = (Mathf.Cos(degree2) * vel2).magnitude;

            var lglooNerfRate = 1f;
            if (hasLglooStun())
            {
                lglooNerfRate = 0.5f;
            }
            //ridbody.AddExplosionForce((otherCollision.forceArgument + m2) + 200 * lglooNerfRate, collision.contacts[0].point, 4);
            //collision.collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce((forceArgument + m1) + 50, collision.contacts[0].point, 4);
            var attackForce = attackForceFeedback.Evaluate(m2);
            var hitForce = hitForceFeedback.Evaluate(m1);
            ridbody.AddExplosionForce((attackForce * bodyCollider.material.staticFriction * ridbody.mass + hitForce * bodyCollider.material.staticFriction * ridbody.mass), collision.contacts[0].point, 2, 0f, ForceMode.Force);

            Debug.LogError($"结算 {otherCollision.gameObject.name} 对 {this.gameObject.name} -> m1 : {m1} m2 : {m2} 别人的attackForce {attackForce} 我的hitForce {hitForce}");

            //如果对方在施法过程里打断施法
            if (otherCollision.skill && otherCollision.isUseSkill)
            {
                otherCollision.skill.ExitUseMode();
            }
        }

    }
    #endregion

}
