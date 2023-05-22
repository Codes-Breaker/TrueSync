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
using System.Linq;

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
    [Header("跳跃间隔")]
    public float jumpFrequency = 1f;

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
    [Header("最大刹车时间")]
    public float maxBreakTime;
    [Header("最小漂移角度")]
    public float minDriftAngle = 30f;


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
    public float maxStunValue = 100;
    [Header("血量最大值")]
    private float maxHPValue = 100;
    [Header("撞击力映射打击数值的曲线")]
    public AnimationCurve forceToHuntCurve;
    [Header("打击恢复时间")]
    public float cureTime;
    [Header("眩晕回复时间")]
    public float stunRecoverTime = 50;
    [Header("速度换算旋转速度参数")]
    public float velocityToRollAngleArgument = 1;
    [Header("眩晕旋转停止时最大角度")]
    public float stunStopRollMaxAngle;
    [Header("眩晕旋转停止时最小角度")]
    public float stunStopRollMinAngle;
    [Header("眩晕时停止转动的最小速度阈值")]
    public float stunStopRollMinVelocity;

    [Header("是否达到最大速度")]
    public bool isAtMaxSpeed = false;
    [Header("动画速度曲线")]
    public AnimationCurve runAnimCurve;
    [Header("动画速度播放曲线")]
    public AnimationCurve runAnimPlayCurve;
    [Header("速度击退曲线")]
    public AnimationCurve hitKnockbackCurve;
    [Header("击退范围上限")]
    public float hitMaxDistance = 5;

    [Header("重力相关")]
    [Header("重力缩放")]
    public float gravityScale = 1;
    [Header("下坠重力缩放")]
    public float fallingGravityScale = 1;
    public float ascendingGravityScale = 1;

    [Space(10)]
    [Header("相关需要关联组件")]
    public Slider gpSlider;
    public Slider hpSlider;
    public Slider stunSlider;
    public Canvas canvas;
    public Rigidbody ridbody;
    public Collider bodyCollider;
    public MeshRenderer ringRenderer;
    private GameController gameController;
    public Animator anima;
    public GameObject IKObject;
    public GameObject stunEffect;
    public GameObject smokeEffect;
    public ParticleSystem particle;
    public SimpleFloatingObject floatObj;
    public GrounderQuadruped grounderQuadruped;

    //攻击力
    public float forceArgument;
    //防御力系数
    [Range(0, 2)]
    public float receivedForceRate = 1;

    //地面法线
    private Vector3 groundNormal;
    public bool isDead = false;
    private bool hasJump = false;
    private bool isJumpFrequency = false;
    private bool hasBrake = false;
    private bool isDrift;
    private bool isWalk;
    private bool isRollContinu;
    public bool isGrounded;
    public bool isStun { get; private set; }
    //起身
    public bool isRecovering { get; private set; }

    //摇杆输入值最小值
    private float movementThrashold = 0.35f;
    public float sensitivity = 0.5f;
    private Vector3 initialRotation;
    private Vector3 forceTarget;
    private Quaternion initialRot;
    // 隐藏参数
    [HideInInspector]
    public float targetAngle;
    [HideInInspector]
    public float currentStunValue;
    public float currentHPValue;
    private float lastHPValue;
    private float maxActorGas = 100;
    [HideInInspector]
    public float currentGas;
    [HideInInspector]
    public bool releasing = false;
    [HideInInspector]
    public float HPtimer;
    [HideInInspector]
    public SkillItemControllerBase skill;
    [HideInInspector]
    public bool isUseSkill;

    int speedUpGas = 0;
    public int maxSpeedUpGas = 5;

    [HideInInspector]
    public Vector3 velocityBeforeCollision = Vector3.zero;
    [HideInInspector]
    public Vector3 positionBeforeCollision = Vector3.zero;

    private float driftAngle;

    [Range(0, 1)]
    public float continueReceivedForceRate = 0.2f;

    public float invulernableTime = 0;
    public bool invulernable = false;
    public bool isTouchingSlope = false;

    private int defaultLayer = 0;

    //public MaterialController meshController;
    //特效资源
    //当前的重力大大小
    public Vector3 currentGravity;

    public List<Buff> buffs = new List<Buff>();

    public int playerIndex = 0;

    //眩晕旋转状态相关
    Vector3 rollRotationAxis = Vector3.zero;
    float rollRotationAmount = 0f;


    public bool isInWater = false;
    private bool hasInWater = false;

    //刹车时的朝向
    private Vector3 initialBrakeTarget = Vector3.zero;


    //本地计时器相关
    private float lastJumpLandTime ;
    private float lastStunTime;
    private float ElapsedRollTime; //过去了的时间
    private float TargetRollTime; //目标时间

    //是否处于眩晕状态
    //起身时间
    [HideInInspector]
    private float recoveryTime = 1;



    private void Awake()
    {
        speedUpGas = maxSpeedUpGas;
        currentStunValue = maxStunValue;
        currentHPValue = maxHPValue;
        isGrounded = true;
        //originalRadius = (bodyCollider as SphereCollider).radius;
        //originalCenter = (bodyCollider as SphereCollider).center;
        defaultLayer = this.gameObject.layer;
        initialRotation = ridbody.transform.rotation.eulerAngles;
        initialRot = ridbody.transform.rotation;
        lastJumpLandTime = jumpFrequency;

        gameController = GameObject.Find("GameManager")?.GetComponent<GameController>();


        smokeEffect.gameObject.SetActive(true);
        particle.Stop();
    }

    private void Start()
    {
        this.ridbody.useGravity = false;
        if(inputReader != null)
            SetControlSelf();
        SetRingColor();
    }

    private void LateUpdate()
    {
        SetSlider();
        MoveRoll();
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
        CheckStun();
        CheckSpeed();
        CheckInVulernable();
        CheckIsGrounded();
        UpdateBuff();
        CheckSlopeAndDirections();
        CheckIsInWater();
        SetGravity();
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

    public void TakeDamage(int number)
    {
        if (isStun)
            return;
        currentStunValue = Math.Max(0, currentStunValue - number);
        CheckStun();
    }

    private void UpdateBuff()
    {
        foreach (var buff in buffs.ToArray())
        {
            if (!buff.isEnd)
                buff.OnBuffUpdate();
            if (buff.isEnd)
            {
                buffs.Remove(buff);
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
        anima.SetFloat("Speed", runAnimCurve.Evaluate(speed));
        anima.SetFloat("playSpeed", runAnimPlayCurve.Evaluate(speed));
        anima.SetFloat("velocityY", ridbody.velocity.y);
        anima.SetBool("Releasing", releasing);
        if (isInWater && !hasInWater)
        {
            anima.SetTrigger("isInWater");
            hasInWater = true;
        }
        else if (!isInWater && hasInWater)
        {
            hasInWater = false;
        }
        anima.SetBool("inWater", isInWater);
        anima.SetBool("isDrift", isDrift);
        anima.SetFloat("driftAngle", driftAngle);
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
        if (ridbody.velocity.y >= 0)
        {
            gravityScale = ascendingGravityScale;
        }
        else if (ridbody.velocity.y < 0)
        {
            gravityScale = fallingGravityScale;
        }
        ridbody.AddForce(Physics.gravity * (gravityScale) * ridbody.mass);
        currentGravity = Physics.gravity * (gravityScale);
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

    private void MoveWalk(Vector2 axisInput, ControlDeviceType controlDeviceType)
    {
        if (isStun)
            return;
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
        if (releasing)
        {
            isWalk = false;
            if (ridbody.velocity.magnitude < runMaxVelocity * 0.96f)
            {
                var acceleration = runMaxVelocity / runSpeedUpTime;
                var forceMagnitude = ridbody.mass * acceleration;
                var gravityDivide = Vector3.zero;
                if (isTouchingSlope || isGrounded)
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
                //Debug.Log($"isTouchingSlope || isGrounded {isTouchingSlope || isGrounded} forceMagnitude {forceMagnitude} velocity {ridbody.velocity} velocityMagnitude {ridbody.velocity.magnitude}");
                CheckisDrift();



                if (axisInput.magnitude > movementThrashold)
                {
                    targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                    transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), runSpeedUpRotationRate);
                }
            }
            else
            {
                CheckisDrift();
                if (axisInput.magnitude > movementThrashold)
                {
                    targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                    transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), runMaxSpeedRotationRate);
                }
               // Debug.Log($"isTouchingSlope || isGrounded {isTouchingSlope || isGrounded} velocity {ridbody.velocity} velocityMagnitude {ridbody.velocity.magnitude}");
            }
        }
        else
        {

            if (ridbody.velocity.magnitude < movementMaxVelocity && axisInput.magnitude > movementThrashold)
            {
                isWalk = true;
                targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                var acceleration = movementMaxVelocity / movementSpeedUpTime;
                var forceMagnitude = ridbody.mass * acceleration;

                var gravityDivide = Vector3.zero;
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
                
                //如果刹车走路状态不在施加推进力
                if(!hasBrake)
                {

                    ridbody.AddForce(moveTarget * forceMagnitude - gravityDivide, ForceMode.Force);
                    transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), movementRotationRate);
                }
            }
            else
            {
                isWalk = false;
            }


        }

    }


    private void MoveBrake(bool brake)
    {
        if (isStun || isInWater)
        {
            if (hasBrake)
            {
                anima.SetBool("isBrake", false);
                hasBrake = false;
            }
            return;
        }
        //只有地面能刹车
        if (!isGrounded && !isTouchingSlope)
        {
            if (hasBrake)
            {
                anima.SetBool("isBrake", false);
                hasBrake = false;
            }
            return;
        }

        if (brake)
        {
            ridbody.AddForce(-ridbody.velocity * decelerationForceArgument);
            ridbody.AddTorque(-ridbody.angularVelocity * decelerationTorqueArgument);
            if (!hasBrake)
            {
                anima.SetBool("isBrake", true);
                hasBrake = true;
                initialBrakeTarget = ridbody.transform.forward;
            }
            anima.SetFloat("TargetAngle", Vector3.SignedAngle(initialBrakeTarget, transform.forward, Vector3.up));
            anima.SetFloat("AbsTargetAngle", Math.Abs(Vector3.Angle(initialBrakeTarget, transform.forward)));
        }
        else
        {
            if (hasBrake)
            {
                anima.SetBool("isBrake", false);
                hasBrake = false;
            }
        }
    }

    private void MoveJump(bool jump)
    {
        if (isStun)
            return;
        if (buffs.Any(x => x is HitBuff))
            return;

        if ((isGrounded || isTouchingSlope) && ridbody.velocity.y <= 0)
        {
            anima.SetBool("OnGround", true);
            anima.SetBool("Jump", false);
        }

        if (isJumpFrequency)
            lastJumpLandTime += Time.deltaTime;

        if (jump && (isGrounded || isTouchingSlope || isInWater) && !hasJump && lastJumpLandTime >= jumpFrequency)
        {
            ridbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            hasJump = true;
            isJumpFrequency = false;
            anima.SetBool("OnGround", false);
            anima.SetBool("Jump", true);
            if (isAtMaxSpeed)
                currentGas = 0;
        }

        if (!jump && (isGrounded || isTouchingSlope || isInWater) && ridbody.velocity.y <= 0)
        {
            if(!isJumpFrequency)
                lastJumpLandTime = 0f;

            isJumpFrequency = true;
            if (hasJump)
                hasJump = false;

        }

    }

    private void UseWeapon(bool isUse)
    {
        if (isUse && skill && currentGas == 0)
        {
            skill.UseSkillItem();
        }
    }

    private void MoveCharge(bool charge)
    {
        if (isStun)
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
            anima.SetBool("Charge", true);
        }
        else
        {
            anima.SetBool("Charge", false);
        }
    }

    private void MoveRelease(bool charge)
    {
        if (isStun)
            return;
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

    private void MoveRoll()
    {
        if (isStun)
        {
            if(ridbody.velocity.magnitude > stunStopRollMinVelocity)
            {
                rollRotationAxis = - Vector3.Cross(groundNormal, ridbody.velocity);
                rollRotationAmount = Vector3.ProjectOnPlane(ridbody.velocity,groundNormal).magnitude * velocityToRollAngleArgument;
                IKObject.transform.Rotate(rollRotationAxis, -rollRotationAmount, Space.Self);
                isRollContinu = true;
            }
            else
            {
                var upAngle = Vector3.Angle(IKObject.transform.up, groundNormal);
                var forwardAngle = Vector3.Angle(IKObject.transform.forward, groundNormal);
                isRollContinu = upAngle < stunStopRollMinAngle || (180f - upAngle) < stunStopRollMinAngle || (stunStopRollMaxAngle < upAngle && upAngle < (180 - stunStopRollMaxAngle))&&(stunStopRollMaxAngle < forwardAngle && forwardAngle < (180 - stunStopRollMaxAngle));
                //近乎停止旋转时的平衡补偿
                if(isRollContinu)
                {
                    IKObject.transform.Rotate(rollRotationAxis, -rollRotationAmount, Space.Self);

                }
            }
        }
    }

    #endregion

    #region Check

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

    private void CheckStun()
    {
        if (isStun && !isRecovering)
        {
            lastStunTime += Time.fixedDeltaTime;
            if (lastStunTime >= stunRecoverTime)
            {
                isRecovering = true;
                currentStunValue = maxStunValue;

                this.transform.DORotate(new Vector3(0, 0, 0), 0.2f).onComplete += () =>
                {
                    isStun = false;
                };
            }
        }
        else
        {
            if (currentStunValue <= 0)
            {
                isStun = true;
                lastStunTime = 0 ;
            }
        }
    }

    private void CheckSpeed()
    {
        //Debug.LogError($"current speed: {velocityBeforeCollision.magnitude} ---> {(velocityBeforeCollision.magnitude / runMaxVelocity) * 100}% {(DateTime.Now - releaseDateTime).TotalSeconds} seconds");
        if (velocityBeforeCollision.magnitude >= runMaxVelocity * 0.9f)
        {
            SetRingMaxColor();
            isAtMaxSpeed = true;
            anima.SetBool("isAtMaxSpeed", isAtMaxSpeed);
            if ((isGrounded || isTouchingSlope) && releasing)
                particle.Play();
        }
        else
        {
            SetRingColor();
            isAtMaxSpeed = false;
            anima.SetBool("isAtMaxSpeed", isAtMaxSpeed);
            particle.Stop();
        }
        if (!isGrounded && !isTouchingSlope && !isStun)
            particle.Stop();
    }

    private void CheckisDrift()
    {
        var velocity = Vector3.ProjectOnPlane(ridbody.velocity, groundNormal);
        var angle = Vector3.Angle(ridbody.transform.forward, velocity);
        driftAngle = Vector3.SignedAngle(ridbody.transform.forward, velocity, Vector3.up);
        if (angle > minDriftAngle)
            isDrift = true;
        else
            isDrift = false;
    }

    private void SetIK()
    {
        var targetIK = isStun ? 0 : 1;

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
        isGrounded = Physics.CheckSphere(bodyCollider.transform.position + (bodyCollider as SphereCollider).center - new Vector3(0, (bodyCollider as SphereCollider).radius, 0), 0.02f, groundMask);

    }

    private void CheckSlopeAndDirections()
    {
        RaycastHit slopeHit;
        isTouchingSlope = false;
        if (Physics.SphereCast(bodyCollider.transform.position + (bodyCollider as SphereCollider).center + transform.forward.normalized * slopeCheckerForwardOffset, slopeCheckerThrashold, Vector3.down, out slopeHit, (bodyCollider as SphereCollider).radius + 0.01f, groundMask))
        {
            groundNormal = slopeHit.normal;

            if (Vector3.Angle(Vector3.up, slopeHit.normal) > miniClimbableSlopeAngle && Vector3.Angle(Vector3.up, slopeHit.normal) < maxClimbableSlopeAngle)
            {
                if (!isGrounded)
                    isTouchingSlope = true;
            }
        }
        else
        {
            groundNormal = Vector3.up;
        }
    }
    private IEnumerator AddExplosiveForceSmooth(float force, Vector3 contactPoint)
    {
        int steps = 0;
        int stepsToTake = 3;
        while (steps < stepsToTake)
        {
            ridbody.AddExplosionForce(force / stepsToTake, contactPoint, 2, 0, ForceMode.Force);
            steps++;
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion
    #region SetUI
    private void SetSlider()
    {
        gpSlider.value = (float)(currentGas / maxActorGas);
        stunSlider.value = (float)(currentStunValue / maxStunValue);
        hpSlider.value = (float)(currentHPValue / maxHPValue);
        canvas.transform.forward = Camera.main.transform.forward;
        gpSlider.transform.position = bodyCollider.transform.position;
        stunSlider.transform.position = bodyCollider.transform.position;
        hpSlider.transform.position = bodyCollider.transform.position;
        gpSlider.transform.localPosition = gpSlider.transform.localPosition + new Vector3(0, 1.25f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
        stunSlider.transform.localPosition = stunSlider.transform.localPosition + new Vector3(0, 1.5f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
        hpSlider.transform.localPosition = hpSlider.transform.localPosition + new Vector3(0, 1.75f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
    }
    private void SetRingColor()
    {
        var rendererBlock = new MaterialPropertyBlock();
        ringRenderer.GetPropertyBlock(rendererBlock, 0);
        rendererBlock.SetColor("_Color", InputReadManager.Instance.playerColors[playerIndex]);
        ringRenderer.SetPropertyBlock(rendererBlock, 0);
    }

    // temp, 告诉用户到达最大速
    private void SetRingMaxColor()
    {
        var rendererBlock = new MaterialPropertyBlock();
        ringRenderer.GetPropertyBlock(rendererBlock, 0);
        rendererBlock.SetColor("_Color", Color.red);
        ringRenderer.SetPropertyBlock(rendererBlock, 0);
    }


    #endregion

    #region Render

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

            Vector3 vel1 = new Vector3(velocityBeforeCollision.x, 0.25f * velocityBeforeCollision.y, velocityBeforeCollision.z);
            Vector3 vel2 = new Vector3(otherCollision.velocityBeforeCollision.x, 0.25f * otherCollision.velocityBeforeCollision.y, otherCollision.velocityBeforeCollision.z);

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
            collision.collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce((forceArgument + m1) * 1.5f + 50, collision.contacts[0].point, 4);
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
            var force = KnockBackForce(1);

            var hitDir = Vector3.ProjectOnPlane((ridbody.position - collision.contacts[0].point), Vector3.up).normalized;
            if (otherCollision.releasing)
                ridbody.AddForce((force) * hitDir, ForceMode.Force);
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

    /// <summary>
    /// 结算击退距离
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    private float KnockBackForce(float distance)
    {
        var gravityDivide = Vector3.ProjectOnPlane(Physics.gravity, groundNormal) * ridbody.mass;
        var gravityFrictionDivide = Physics.gravity - gravityDivide;
        var frictionForceAcceleration =  bodyCollider.material.dynamicFriction * gravityFrictionDivide.magnitude;

        var desiredV0 = (float)Math.Sqrt((2 * frictionForceAcceleration * distance));
        var acceleration = desiredV0 / Time.fixedDeltaTime;

        return acceleration * ridbody.mass;
    }

    /// <summary>
    /// 在空中的击退距离
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    private float KnockBackOnAirForce(float distance)
    {
        var currentVelocity = ridbody.velocity;
        float forceMagnitude = 0f;
        if(currentVelocity.y > 0)
        {
            //上升状态处理
            var riseTime = currentVelocity.y / currentGravity.magnitude;
            var dropHeight = ridbody.transform.position.y + (bodyCollider as SphereCollider).center.y + currentGravity.magnitude * Mathf.Pow(riseTime, 2.0f);
            var dropTime = Mathf.Sqrt(2 * currentGravity.magnitude * dropHeight);
            var initialVelocity = distance / (dropTime+ riseTime);
            var acceleration = initialVelocity / Time.fixedDeltaTime;
            forceMagnitude = acceleration * ridbody.mass;

        }
        else
        {
            //下降状态处理
            var riseTime = Mathf.Abs(currentVelocity.y) / currentGravity.magnitude;
            var dropHeight = ridbody.transform.position.y + (bodyCollider as SphereCollider).center.y + currentGravity.magnitude * Mathf.Pow(riseTime, 2.0f);
            var dropMaxTime = Mathf.Sqrt(2 * currentGravity.magnitude * dropHeight);
            var dropTime = dropMaxTime - riseTime;


            var gravityDivide = Vector3.ProjectOnPlane(Physics.gravity, groundNormal) * ridbody.mass;
            var gravityFrictionDivide = Physics.gravity - gravityDivide;
            var frictionForceAcceleration = bodyCollider.material.dynamicFriction * gravityFrictionDivide.magnitude;

            var initialVelocity = (float)(((distance) + frictionForceAcceleration * Mathf.Pow(riseTime, 2)/2)/(0.4 * dropTime + 0.6 * riseTime));
            forceMagnitude = initialVelocity / Time.fixedDeltaTime * ridbody.mass;
        }
        return forceMagnitude;
    }
    Vector3 knockingPosition = Vector3.zero;

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var buff in buffs)
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
            collision.collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce((forceArgument + m1) * 2f + 250, collision.contacts[0].point, 4);
        }

        if (collision.gameObject.GetComponent<CharacterContorl>())
        {
            //特效
            var eventObjectPrefab = Resources.Load<GameObject>("MediumHit");
            var eventObjectGameObject = Instantiate(eventObjectPrefab, collision.contacts[0].point, Quaternion.Euler(new Vector3(0, 0, 0)));

            var otherCollision = collision.gameObject.GetComponent<CharacterContorl>();
            //自身速度
            Vector3 velocitySelf = new Vector3(velocityBeforeCollision.x, velocityBeforeCollision.y, velocityBeforeCollision.z);
            velocitySelf = Vector3.ProjectOnPlane(velocitySelf, groundNormal).normalized * velocitySelf.magnitude;
            //对方速度
            Vector3 velocityOther = new Vector3(otherCollision.velocityBeforeCollision.x, otherCollision.velocityBeforeCollision.y, otherCollision.velocityBeforeCollision.z);
            velocityOther = Vector3.ProjectOnPlane(velocityOther, groundNormal).normalized * velocityOther.magnitude;

            Vector3 cPoint = collision.contacts[0].point;
            Vector3 contactToMe = cPoint - positionBeforeCollision;
            Vector3 contactToOther = cPoint - otherCollision.positionBeforeCollision;

            var angleSelf = Vector3.Angle(velocitySelf, contactToMe);
            var angleOther = Vector3.Angle(velocityOther, contactToOther);
            var degreeSelf = angleSelf * Mathf.Deg2Rad;
            var degreeOther = angleOther * Mathf.Deg2Rad;

            Vector3 impactVelocity = collision.relativeVelocity;
            var momentumSelf = (Mathf.Cos(degreeSelf) * velocitySelf).magnitude;
            var momentumOther = (Mathf.Cos(degreeOther) * velocityOther).magnitude;

            var lglooNerfRate = 1f;
            if (hasLglooStun())
            {
                lglooNerfRate = 0.5f;
            }
            //出招加成
            var hasBuff = (otherCollision.isAtMaxSpeed && !otherCollision.isGrounded) ? 1.1f : 1;
            var myBuff = isAtMaxSpeed && !otherCollision.isGrounded ? 1.1f : 1;

            var hitDir = Vector3.ProjectOnPlane((ridbody.position - collision.contacts[0].point), groundNormal).normalized;
            var myHitKnockback = hitKnockbackCurve.Evaluate(momentumSelf * myBuff + 2);
            var otherHitKnockback = hitKnockbackCurve.Evaluate(momentumOther * hasBuff);
            var hitDistance = Math.Min(hitMaxDistance, otherHitKnockback + myHitKnockback);

            float force = 0f;
            //施加水平推力
            if (isGrounded || isTouchingSlope)
				force = KnockBackForce(Math.Min(hitMaxDistance, hitKnockbackCurve.Evaluate(momentumOther * hasBuff) + hitKnockbackCurve.Evaluate(momentumSelf * myBuff + 2)));
            else
                force = KnockBackOnAirForce(Math.Min(hitMaxDistance, hitKnockbackCurve.Evaluate(momentumOther * hasBuff) + hitKnockbackCurve.Evaluate(momentumSelf * myBuff + 2)));
           
            var hitOnPlane = Vector3.ProjectOnPlane((collision.contacts[0].point - ridbody.position), groundNormal).normalized;
            var forwardOnPlane = Vector3.ProjectOnPlane(ridbody.transform.forward, groundNormal).normalized;
            var hitAngle = Vector3.Angle(forwardOnPlane, hitOnPlane);
            anima.SetFloat("hitAngle", hitAngle);
            if (myHitKnockback > 1 || otherHitKnockback > 1)
            {
                anima.SetBool("isHit", true);
            }

            //打击眩晕和血量
            TakeDamage((int)(hitKnockbackCurve.Evaluate(momentumOther * hasBuff) * 10));

            ridbody.AddForce((force)* hitDir, ForceMode.Force);

            //施加转角力 正值顺时针转动，负值逆时针转动
            var torgueAngle = Vector3.SignedAngle(velocityOther, contactToOther, groundNormal);
            if (torgueAngle >= 0)
            {
                ridbody.AddRelativeTorque(Vector3.right * velocityOther.magnitude * Mathf.Cos(torgueAngle * Mathf.Deg2Rad), ForceMode.Force);
            }
            else
            {
                ridbody.AddRelativeTorque(Vector3.left * velocityOther.magnitude * Mathf.Cos(torgueAngle * Mathf.Deg2Rad), ForceMode.Force);
            }

            //Debug.LogError($"结算 {otherCollision.gameObject.name} force {force} hit Dir {hitDir}");
            knockingPosition = this.transform.position;
            var buff = new HitBuff(this);
            this.buffs.Add(buff);

            //如果对方在施法过程里打断施法
            if (otherCollision.skill && otherCollision.isUseSkill)
            {
                otherCollision.skill.ExitUseMode();
            }

            //旋转
            if (isStun)
            {
                var targetTime = hitKnockbackCurve.Evaluate(momentumOther * hasBuff);
                TargetRollTime = Math.Max(targetTime, TargetRollTime);
                if (TargetRollTime - ElapsedRollTime < targetTime)
                {
                    TargetRollTime += (targetTime - (TargetRollTime - ElapsedRollTime));
                }
            }
        }

    }
    #endregion

}
