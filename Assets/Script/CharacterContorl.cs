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
using BoingKit;
using Collision = UnityEngine.Collision;
using XFurStudio3.Core;

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
    [Header("攀爬速度（和水平速度的比值）")]
    public float stepSpeedArgument;
    [Space(10)]
    [Header("跳跃力系数")]
    public float jumpForce;
    [Header("跳跃间隔")]
    public float jumpFrequency = 1f;
    [Header("前扑间隔")]
    public float jumpRushFrequency = 1f;
    [Header("跳跃加成")]
    public float jumpBonusToVelocity = 1.4f;
    [Header("走路加成")]
    public float jumpWalkBonusToVelocity = 1.2f;
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
    [Header("刹车时的转向Lerp系数")]
    public float breakRotationRate = 0.02f;
    [Header("体力消耗时间")]
    public float chargeTime;
    [Header("体力回复时间")]
    public float recoverTime;

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
    [Header("攀爬面检测点向前偏移")]
    public float stepCheckerForwardOffset = 0.2f;
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
    [Header("眩晕倍数")]
    public float stunAccumulateTime = 1.2f;
    [Header("速度换算旋转速度参数")]
    public float velocityToRollAngleArgument = 1;
    [Header("眩晕旋转停止时最大角度")]
    public float stunStopRollMaxAngle;
    [Header("眩晕旋转停止时最小角度")]
    public float stunStopRollMinAngle;
    [Header("眩晕时停止转动的最小速度阈值")]
    public float stunStopRollMinVelocity;
    [Header("打击加成系数")]
    public float hitKnockBackToOtherArgument = 1f;
    [Header("自身击退加成系数")]
    public float hitKnockbackToSelfArgument = 1f;
    [Header("正面受击补偿")]
    public float hitBonusToHead = 1f;
    [Header("侧面受击补偿")]
    public float hitBonusToSide = 1f;
    [Header("后面受击补偿")]
    public float hitBonusToBack = 1f;

    [Header("是否达到最大速度")]
    public bool isAtMaxSpeed = false;
    [Header("是否达到走路速度")]
    public bool isAtWalkSpeed = false;
    [Header("动画速度曲线")]
    public AnimationCurve runAnimCurve;
    [Header("动画速度播放曲线")]
    public AnimationCurve runAnimPlayCurve;
    [Header("速度击退曲线")]
    public AnimationCurve hitKnockbackCurve;
    [Header("自身速度击退取消")]
    public AnimationCurve hitKnockbackSelfCurve;
    [Header("击退范围上限")]
    public float hitMaxDistance = 5;

    [Header("重力相关")]
    [Header("重力缩放")]
    public float gravityScale = 1;
    [Header("下坠重力缩放")]
    public float fallingGravityScale = 1;
    public float ascendingGravityScale = 1;

    [Header("出招加成")]
    public float buffAttack = 2f;
    [Header("打击距离换算眩晕")]
    public float distanceToStunCoef = 10f;
    [Header("打击距离换算血量")]
    public float distanceToHPCoef = 2.5f;

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
    public GameObject smokeEffect;
    public GameObject impactEffect;
    public ParticleSystem particle;
    public SimpleFloatingObject floatObj;
    public GrounderQuadruped grounderQuadruped;
    public LookAtIK lookAtIK;
    public RagdollActivator ragdollController;
    public CinemachineTargetGroup cinemachineTargetGroup;
    public Camera m_camera;
    [Header("背部挂点")]
    public Transform itemPlace;
    [Header("头部挂点")]
    public Transform itemPlaceHead;
    [Header("头部挂点不带物理")]
    public Transform itemPlaceHeadStatic;
    [Header("尾巴挂点")]
    public Transform itemPlaceTail;
    [Header("腰部挂点")]
    public Transform itemPlaceBelly;
    [Header("手部挂点")]
    public Transform itemPlaceHand;
    private Vector2 axisInput;
    public GameObject playerIndicator;
    public TMPro.TMP_Text playerIndexText;
    //攻击力
    public float forceArgument;
    //防御力系数
    [Range(0, 2)]
    public float receivedForceRate = 1;

    //地面法线
    public Vector3 groundNormal;
    public bool isDead = false;
    private bool hasJump = false;
    private bool isJumpFrequency = false;
    private bool hasBrake = false;
    private bool isDrift;
    private bool isWalk;
    public bool isAirWalk;
    private bool hasAirWalk;
    public bool isGrounded;
    private Vector3 wallNormal;
    private bool isTouchingStep;
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
    public float maxActorStamina = 100;
    [HideInInspector]
    public float currentStamina;
    [HideInInspector]
    public bool releasing = false;
    [HideInInspector]
    public float HPtimer;
    [HideInInspector]
    public ItemAbilityBase itemAbility;
    //[HideInInspector]
    //public SkillItemControllerBase skill;
    //[HideInInspector]
    //public bool isUseSkill;

    int speedUpGas = 0;
    public int maxSpeedUpGas = 5;

    [HideInInspector]
    public Vector3 velocityBeforeCollision = Vector3.zero;
    [HideInInspector]
    public Vector3 positionBeforeCollision = Vector3.zero;

    private float driftAngle;

    [Range(0, 1)]
    public float continueReceivedForceRate = 0.2f;

    public bool invulernable {
        get => buffs.Any(x => x is InvulernableBuff);
    }

    public bool isTouchingSlope = false;

    private int defaultLayer = 0;

    //public MaterialController meshController;
    //特效资源
    //当前的重力大大小
    public Vector3 currentGravity;

    public List<Buff> buffs = new List<Buff>();

    public int playerIndex = 1;


    public bool isInWater = false;
    private bool hasInWater = false;

    public bool isInRocket = false;

    //刹车时的朝向
    private Vector3 initialBrakeTarget = Vector3.zero;

    //本地计时器相关
    private float lastJumpLandTime;
    private float lastJumpRushTime;
    private float lastStunTime;
    private float ElapsedRollTime; //过去了的时间
    //private float TargetRollTime; //目标时间
    private float lastInWaterTime; //上次入水时间
    private float lastCollisionTime; //上次碰撞时间
    private float lastHPSubtractTime = 0; //上次扣血时间
    private float lastHPRecoveryTime = 0; //上次回血时间
    private float lastStunRecoveryTime = 0; //上次眩晕回复时间
    private float lastSpeedUpTime = 0; //上次加速时间
    private float elapsedChargeTime = 0; //已经加速了的时间
    private float lastSlipReadyTime = 0; //上次打滑准备的时间

    //buff控制相关参数
    private bool controlKeepRuning;

    //起身时间
    [HideInInspector]
    private float recoveryTime = 1;
    [Header("回血间隔时间")]
    public float hpRecoveryFrequency = 1;
    [Header("回血延时时间")]
    public float damageRecoveryFrequency = 5;
    [Header("回血量")]
    public float HPRecoveryRate = 2;
    [Header("扣血量")]
    public float HPSubtractRate = 5;
    [Header("扣血间隔时间")]
    public float hpSubtractFrequency = 1;
    [Header("入水扣血延时时间")]
    public float inWaterSubtractFrequency = 3;
    [Header("眩晕回复间隔时间")]
    public float stunRecoveryFrequency = 1;
    [Header("眩晕回复量")]
    public float stunRecoveryRate = 2;
    [Header("眩晕回血延时时间")]
    public float withoutCollidingRecoveryFrequency = 3;

    //是否处于碰撞角色
    private bool isCollidingCharacter = false;
    [Header("眩晕上限减少")]
    public float stunDecreaseRate = 10;
    [Header("眩晕最小值")]
    public float stunMinValue = 50;
    [Header("场景物体距离衰减系数")]
    [Range(0, 1)]
    public float InteractiveDistanceCoef = 1f;
    [Range(0, 1)]
    public float InteractiveStunDistanceCoef = 1f;
    [Header("场景物体反馈速度根据角色撞击速度")]
    public AnimationCurve speedToInteractiveEffect;

    public GameObject crown;

    public GameObject stun;

    [Header("濒死状态提示血量")]
    public float dangerHpTip = 20f;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public AnimationCurve blinkCurve;

    private bool isRecoveringFromStun = false;

    //体力完全回复中
    private bool fullyRecoveringStamina;

    public Color staminaNormalColor;
    public Color staminaRecoveringColorYellow;
    public Color staminaRecoveringColorRed;
    public Image staminaFillImage;
    public GameObject stunButtonHint;
    public Image stunProgress;

    [Header("最小体力消耗值")]
    public float minStaminaThreshold = 10f;
    [Header("跳跃空中转角")]
    public float jumpRotateAngle = 60f;
    [Header("打滑保护帧")]
    public int slipProtectFrame = 3;
    private float slipProtectTime;
    private bool canSlip = false;
    public bool isBrake = false;
    public RangeDector rangeDector;
    public RangeDector viewRangeDector;
    private CharacterContorl lastSeenTarget;
    private CharacterContorl lastSeenView;
    private List<SnowGroundDetector> snowDectors;

    public AnimationEventReceiver animationEventReceiver;
    public BoingBones boingBones;
    //毛发数据
    public FurData furData;
    public XFurStudioInstance xfurInstance;
    public int furIndex;
    [ColorUsage(true, true)]
    public Color dangerHPColor;
    [ColorUsage(true, true)]
    public Color normalHPColor;
    [ColorUsage(true, true)]
    public Color invulenableHPColor;

    private void Awake()
    {
        snowDectors = GetComponentsInChildren<SnowGroundDetector>().ToList();
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
        slipProtectTime = slipProtectFrame * Time.fixedDeltaTime;
        gameController = GameObject.Find("GameManager")?.GetComponent<GameController>();
        crown.gameObject.SetActive(false);

        //impactEffect.gameObject.SetActive(false);
        smokeEffect.gameObject.SetActive(true);
        particle.Stop();
        crown.gameObject.SetActive(false);
        ragdollController.Ragdoll(false, Vector3.zero);
        stunProgress.gameObject.SetActive(false);
        stunButtonHint.gameObject.SetActive(false);
    }

    private void Start()
    {
        this.ridbody.useGravity = false;
        currentStamina = maxActorStamina;
        if (inputReader != null)
            SetControlSelf();
        SetRingColor();

    }

    private void LateUpdate()
    {
        SetSlider();
        SetAnimState();
        SetQTE();
        //MoveRoll();
        SetHPHint();
        SetGPHint();
        LateUpdateBuff();
        SetIK();
        SetView();
        //SetEffect();
    }

    private void Update()
    {
        SetAnimatorArgument();
        TickItemAbilityUpdate();
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            velocityBeforeCollision = GetComponent<Rigidbody>().velocity;
            positionBeforeCollision = GetComponent<Rigidbody>().position;
        }

        CheckSlipery();
        CheckStun();
        CheckSpeed();
        CheckIsGrounded();
        UpdateBuff();
        CheckSlopeAndDirections();
        ClimbStep();
        //CheckSlopeAndDirections();
        CheckIsInWater();
        UpdateHP();
        UpdateStunRecovery();
        SetGravity();
        UpdateStamina();
        RecordCollisionVelocity();
        TickItemAbilityFixedUpdate();
    }

    //驱动道具的Update
    private void TickItemAbilityUpdate()
    {
        if (itemAbility != null)
        {
            itemAbility.Update();
        }
    }

    //驱动道具的FixedUpdate
    private void TickItemAbilityFixedUpdate()
    {
        if(itemAbility != null)
        {
            itemAbility.FixedUpdate();
        }
    }

    public void SetControlSelf()
    {
        inputReader.moveAciotn = MoveWalk;
        inputReader.chargeAction = MoveCharge;
        inputReader.interactWeaponAction = UseItem;
        inputReader.jumpAction = MoveJump;
        inputReader.brakeAciton = MoveBrake;
    }


    private void SetDead(Vector3 hitDir)
    {
        stun.gameObject.SetActive(false);
        //stunProgress.gameObject.SetActive(false);
        isDead = true;
        anima.enabled = false;
        ragdollController.Ragdoll(true, hitDir);
        Destroy(floatObj);
        Dead();
    }

    public void SetWin()
    {
        crown.gameObject.SetActive(true);
        inputReader.moveAciotn = null;
        inputReader.chargeAction = null;
        inputReader.interactWeaponAction = null;
        inputReader.jumpAction = null;
        inputReader.brakeAciton = null;
        crown.transform.DOLocalRotate(new Vector3(crown.transform.localRotation.eulerAngles.x, 360, 0), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    public void TakeStun(int number)
    {
        if (HasDamageImmuneBuff())
            return;
        if (HasRollStun() || invulernable)
            return;
        currentStunValue = Math.Max(0, currentStunValue - number);
        CheckStun();
    }

    public void GainItemAbility(ItemAbilityBase itemAbility)
    {
        this.itemAbility = itemAbility;
        this.itemAbility.Init();
    }

    public void RemoveItemAbility()
    {
        this.itemAbility = null;
    }



    public void TakeDamage(float number, Vector3 hitDir)
    {
        if (HasDamageImmuneBuff())
            return;
        //锁血操作
        if (currentHPValue > dangerHpTip && Math.Max(currentHPValue - number, 0) == 0)
        {
            currentHPValue = 1;
        }
        else
        {
            currentHPValue -= number;
            currentHPValue = Math.Max(currentHPValue, 0);
        }

        lastHPSubtractTime = 0;
        if (currentHPValue == 0)
        {
            SetDead(hitDir);
        }
    }

    private void LateUpdateBuff()
    {
        foreach (var buff in buffs.ToArray())
        {
            if (!buff.isEnd)
                buff.OnBuffLateUpdate();
        }
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
                SetBuffControlState();
            }
        }
    }

    private void UpdateStunRecovery()
    {
        if (isDead)
            return;
        if (!isCollidingCharacter)
            lastCollisionTime += Time.fixedDeltaTime;
        if (!isCollidingCharacter)
        {
            if (lastCollisionTime > withoutCollidingRecoveryFrequency)
            {
                if (lastStunRecoveryTime > stunRecoveryFrequency)
                {
                    if (currentStunValue < maxStunValue)
                    {
                        currentStunValue += stunRecoveryRate;
                        currentStunValue = Math.Min(currentStunValue, maxStunValue);
                        lastStunRecoveryTime = 0;
                    }
                }
                else
                {
                    lastStunRecoveryTime += Time.fixedDeltaTime;
                }
            }

        }

    }

    private void UpdateStamina()
    {
        if (releasing)
            return;
        currentStamina = currentStamina + maxActorStamina / recoverTime * Time.fixedDeltaTime;
        currentStamina = Math.Min(currentStamina, maxActorStamina);
        if (currentStamina == maxActorStamina)
        {
            fullyRecoveringStamina = false;
        }
    }

    private void UpdateHP()
    {
        if (gameController.isGameOver)
            return;
        if (isDead)
            return;

        if (isInWater)
        {
            //入水扣血判定
            if (lastInWaterTime > inWaterSubtractFrequency)
            {
                if (lastHPSubtractTime > hpSubtractFrequency)
                {
                    TakeDamage(HPSubtractRate, Vector3.zero);
                }
                else
                {
                    lastHPSubtractTime += Time.fixedDeltaTime;
                }
            }
        }
        else
        {
            if (lastHPSubtractTime > damageRecoveryFrequency)
            {
                if (currentHPValue < maxHPValue)
                {
                    if (lastHPRecoveryTime > hpRecoveryFrequency)
                    {
                        currentHPValue += HPRecoveryRate;
                        currentHPValue = Math.Min(currentHPValue, maxHPValue);
                        lastHPRecoveryTime = 0;
                    }
                    else
                    {
                        lastHPRecoveryTime += Time.fixedDeltaTime;
                    }
                }
            }
            else
            {
                lastHPSubtractTime += Time.fixedDeltaTime;
            }
        }
    }

    //private void SetEffect()
    //{
    //    if (isAtMaxSpeed)
    //    {
    //        impactEffect.gameObject.SetActive(true);
    //        impactEffect.transform.forward = ridbody.velocity.normalized;
    //    }
    //    else
    //    {
    //        impactEffect.gameObject.SetActive(false);
    //    }
    //}

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
        if (isDead)
            return;
        var speed = new Vector3(ridbody.velocity.x, 0, ridbody.velocity.z).magnitude;
        anima.SetFloat("Speed", runAnimCurve.Evaluate(speed));
        anima.SetFloat("playSpeed", runAnimPlayCurve.Evaluate(speed));
        anima.SetFloat("velocityY", ridbody.velocity.y);
        anima.SetBool("Releasing", releasing||controlKeepRuning);
        anima.SetBool("isAirWalk", isAirWalk);
        if (!hasAirWalk && isAirWalk)
        {
            anima.SetTrigger("AirWalk");
            hasAirWalk = true;
        }
        else if (!isAirWalk && hasAirWalk)
        {
            hasAirWalk = false;
        }

        if (isInWater && !hasInWater && !isInRocket)
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

    IEnumerator DelayRemoveCinemachineTargetGroup()
    {
        yield return new WaitForSeconds(3);
        var cinemachineTargetGroup = GameObject.FindObjectOfType<CinemachineTargetGroup>();
        cinemachineTargetGroup.RemoveMember(transform);
    }

    private void Dead()
    {
        StartCoroutine(DelayRemoveCinemachineTargetGroup());
        //this.gameObject.SetActive(false);
        this.canvas.gameObject.SetActive(false);
        gameController.CheckGameState();
    }

    private void SetGravity()
    {
        if (isDead)
            return;
        //if (ridbody.velocity.y >= 0)
        //{
        //    gravityScale = ascendingGravityScale;
        //}
        //else if (ridbody.velocity.y < 0)
        //{
        //    gravityScale = fallingGravityScale;
        //}
        ridbody.AddForce(Physics.gravity * (gravityScale) * ridbody.mass);
        currentGravity = Physics.gravity * (gravityScale);
    }


    #region Move

    public int countRocketBuff()
    {
        return buffs.Count(x => x is RocketThrusterBuff);
    }

    public List<RocketThrusterBuff> getRocketThrusterBuffs()
    {
        return buffs.Where(x => x is RocketThrusterBuff).Cast<RocketThrusterBuff>().ToList();
    }

    public void FinishRocketThrusterBuffs()
    {
        buffs.ForEach(x =>
        {
            if (x is RocketThrusterBuff)
            {
                x.Finish();
            }
        });
    }

    private void ClimbStep()
    {
        RaycastHit stepHit;
        if (Physics.SphereCast(bodyCollider.transform.position + (bodyCollider as SphereCollider).center * bodyCollider.transform.localScale.y + -groundNormal.normalized * stepCheckerForwardOffset, slopeCheckerThrashold, transform.forward, out stepHit, (bodyCollider as SphereCollider).radius * bodyCollider.transform.localScale.y + 0.02f, groundMask))
        {
            isTouchingStep = true;
            wallNormal = stepHit.normal;
        }
        else
        {
            isTouchingStep = false;
            wallNormal = Vector3.zero;
        }

    }

    public int countRopeStunBuff()
    {
        return buffs.Count(x => x is QTERopeStun);
    }

    public int countQTEStun()
    {
        return buffs.Count(x => x is QTEBuff);
    }

    public int countLargeBuffType()
    {
        return buffs.Count(x => x is LargementPotionBuff);
    }

    public bool isBrakeRecover()
    {
        return anima.GetBool("brakeRecover");
    }

    public bool hasStunBuff()
    {
        var hasStunBuff = buffs.Any(x => x is StunBuff);
        return hasStunBuff;
    }

    public void RemoveRocketBuff()
    {
        foreach (var buff in buffs)
        {
            if (buff is RocketThrusterBuff)
            {
                buff.Finish();
            }
        }
    }
    public void RemoveSliperyBuff()
    {
        foreach(var buff in buffs)
        {
            if (buff is SliperyBuff)
            {
                buff.Finish();
            }
        }
    }

    private void MoveWalk(Vector2 axisInput, ControlDeviceType controlDeviceType)
    {
        if (HasQTEStun() || isDead)
            return;
        //单位化输入方向
        if (controlDeviceType == ControlDeviceType.Mouse)
        {
            Vector3 screenPosition = new Vector3();
            if (gameController.screenMode == ScreenMode.Same)
                screenPosition = gameController.mainCamera.WorldToScreenPoint(transform.position);
            else if(gameController.screenMode == ScreenMode.Split)
                screenPosition = m_camera.WorldToScreenPoint(transform.position);

            var detalPosition = axisInput - new Vector2(screenPosition.x, screenPosition.y);
            axisInput = detalPosition.normalized;
        }
        else
            axisInput = axisInput.normalized;

        if (axisInput.magnitude >= movementThrashold)
        {
            isWalk = true;
        }
        else
        {
            isWalk = false;
        }
        //
        this.axisInput = axisInput;
        if (releasing)
        {
            anima.SetBool("isBrake", false);
            if (ridbody.velocity.magnitude < runMaxVelocity * 0.96f)
            {
                var acceleration = runMaxVelocity / runSpeedUpTime;
                var forceMagnitude = ridbody.mass * acceleration;
                var gravityDivide = Vector3.zero;
                if (isTouchingStep)
                {
                    gravityDivide = Vector3.ProjectOnPlane(Physics.gravity, wallNormal) * ridbody.mass;
                    var gravityFrictionDivide = Physics.gravity - gravityDivide;
                    var frictionForceMagnitude = ridbody.mass * bodyCollider.material.dynamicFriction * gravityFrictionDivide.magnitude;
                    forceMagnitude = forceMagnitude + frictionForceMagnitude;

                }
                else if (isTouchingSlope || isGrounded)
                {
                    gravityDivide = Vector3.ProjectOnPlane(Physics.gravity, groundNormal) * ridbody.mass;
                    var gravityFrictionDivide = Physics.gravity - gravityDivide;
                    var frictionForceMagnitude = ridbody.mass * bodyCollider.material.dynamicFriction * gravityFrictionDivide.magnitude;
                    forceMagnitude = forceMagnitude + frictionForceMagnitude;
                }
                

                //补偿重力分量
                var moveTarget = ridbody.transform.forward;
                moveTarget = moveTarget.normalized;
                if (isTouchingStep)
                    moveTarget = Vector3.ProjectOnPlane(moveTarget + new Vector3(0, stepSpeedArgument, 0), wallNormal).normalized;
                else
                    moveTarget = Vector3.ProjectOnPlane(moveTarget, groundNormal).normalized;
                if (!hasStunBuff())
                    ridbody.AddForce(moveTarget * forceMagnitude - gravityDivide, ForceMode.Force);
                //Debug.Log($"isTouchingSlope || isGrounded {isTouchingSlope || isGrounded} forceMagnitude {forceMagnitude} velocity {ridbody.velocity} velocityMagnitude {ridbody.velocity.magnitude}");
                CheckisDrift();

                if (axisInput.magnitude > movementThrashold)
                {
                    if (gameController.screenMode == ScreenMode.Same)
                        targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + gameController.mainCamera.transform.eulerAngles.y;
                    else if(gameController.screenMode == ScreenMode.Split)
                        targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + m_camera.transform.eulerAngles.y;

                    if (hasBrake)
                        transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), breakRotationRate);
                    else
                        transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), runSpeedUpRotationRate);
                }
            }
            else
            {
                CheckisDrift();
                if (axisInput.magnitude > movementThrashold)
                {
                    if (gameController.screenMode == ScreenMode.Same)
                        targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + gameController.mainCamera.transform.eulerAngles.y;
                    else if (gameController.screenMode == ScreenMode.Split)
                        targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + m_camera.transform.eulerAngles.y;

                    if (hasBrake)
                        transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), breakRotationRate);
                    else
                        transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), runMaxSpeedRotationRate);
                }
                // Debug.Log($"isTouchingSlope || isGrounded {isTouchingSlope || isGrounded} velocity {ridbody.velocity} velocityMagnitude {ridbody.velocity.magnitude}");
            }
        }
        else if (controlKeepRuning)
        {
            if (axisInput.magnitude > movementThrashold)
            {
                CheckisDrift();
                if (gameController.screenMode == ScreenMode.Same)
                    targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + gameController.mainCamera.transform.eulerAngles.y;
                else if (gameController.screenMode == ScreenMode.Split)
                    targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + m_camera.transform.eulerAngles.y;

                if (hasBrake)
                    transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), breakRotationRate);
                else
                    transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), runSpeedUpRotationRate);
            }
        }
        else
        {
            if (ridbody.velocity.magnitude < movementMaxVelocity && axisInput.magnitude > movementThrashold)
            {
                if (gameController.screenMode == ScreenMode.Same)
                    targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + gameController.mainCamera.transform.eulerAngles.y;
                else if (gameController.screenMode == ScreenMode.Split)
                    targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + m_camera.transform.eulerAngles.y;

                var acceleration = movementMaxVelocity / movementSpeedUpTime;
                var forceMagnitude = ridbody.mass * acceleration;

                var gravityDivide = Vector3.zero;
                if (isTouchingStep)
                {
                    gravityDivide = Vector3.ProjectOnPlane(Physics.gravity, wallNormal) * ridbody.mass;
                    var gravityFrictionDivide = Physics.gravity - gravityDivide;
                    var frictionForceMagnitude = ridbody.mass * bodyCollider.material.dynamicFriction * gravityFrictionDivide.magnitude;
                    forceMagnitude = forceMagnitude + frictionForceMagnitude;

                }
                else if(isTouchingSlope || isGrounded)
                {
                    gravityDivide = Vector3.ProjectOnPlane(Physics.gravity, groundNormal) * ridbody.mass;
                    var gravityFrictionDivide = Physics.gravity - gravityDivide;
                    var frictionForceMagnitude = ridbody.mass * bodyCollider.material.dynamicFriction * gravityFrictionDivide.magnitude;
                    forceMagnitude = forceMagnitude + frictionForceMagnitude;
                }
                var moveTarget = ridbody.transform.forward;
                moveTarget = moveTarget.normalized;
                if (isTouchingStep)
                    moveTarget = Vector3.ProjectOnPlane(moveTarget + new Vector3(0, stepSpeedArgument, 0), wallNormal).normalized;
                else
                    moveTarget = Vector3.ProjectOnPlane(moveTarget, groundNormal).normalized;
                RemoveSliperyBuff();
                //如果刹车走路状态不在施加推进力
                if (!hasBrake)
                {
                    if (!hasStunBuff())
                    { 
                        ridbody.AddForce(moveTarget * forceMagnitude - gravityDivide, ForceMode.Force);
                    }
                    transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), movementRotationRate);
                }
                anima.SetBool("isBrake", false);
            }
        }

    }


    private void MoveBrake(bool brake)
    {
        if (isDead)
            return;
        isBrake = brake;
        if (HasRollStun() || isInWater)
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

    private void MoveQTE(QTEButton e)
    {
        //当前QTE
        var currentQTE = this.buffs.Where(x => x is QTEBuff).Cast<QTEBuff>().ToList();
        var chosenOne = currentQTE.FirstOrDefault(x => x.QTEPriority == -1);
        if (chosenOne != null)
        {
            chosenOne.PressButton(e);
            return;
        }
        if (currentQTE.Count > 0)
        {
            //同层级同时结算
            var maxP = currentQTE.Max(x => x.QTEPriority);
            var chosens = currentQTE.Where(x => x.QTEPriority == maxP);
            foreach(var chose in chosens)
            {
                chose.PressButton(e);
            }
        }


    }

    private void MoveJump(bool jump)
    {
        if (jump)
        {
            MoveQTE(QTEButton.A);
        }

        if ((isGrounded || isTouchingSlope) && ridbody.velocity.y <= 0)
        {
            anima.SetBool("Jump", false);
        }
        //if (isStun)
        //{
        //    if (jump)
        //    {
        //        lastStunTime += 0.05f;
        //    }
        //    return;
        //}

        if (isDead)
            return;

        if (hasStunBuff() || isInRocket)
            return;
        if (isJumpFrequency)
            lastJumpLandTime += Time.deltaTime;
        lastJumpRushTime += Time.fixedDeltaTime;
        if (jump && (isGrounded || isTouchingSlope || isInWater) && !hasJump && lastJumpLandTime >= jumpFrequency)
        {
            canSlip = false;
            anima.SetBool("isBrake", false);
            ridbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            hasJump = true;
            isJumpFrequency = false;
            anima.SetBool("Jump", true);
            if (gameController.screenMode == ScreenMode.Same)
                targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + gameController.mainCamera.transform.eulerAngles.y;
            else if(gameController.screenMode == ScreenMode.Split)
            {
                targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + m_camera.transform.eulerAngles.y;

            }
            if (isAtMaxSpeed && lastJumpRushTime > jumpRushFrequency)
            {
                //fullyRecoveringStamina = true;
                var addVelocityValue = (jumpBonusToVelocity - 1) * ridbody.velocity.magnitude;
                var target = Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation) * Vector3.forward;
                var force = addVelocityValue * target.normalized;
                var addForceValue = ridbody.mass * force / Time.fixedDeltaTime;
                ridbody.AddForce(addForceValue, ForceMode.Force);                
                lastJumpRushTime = 0;

            }
            else if (isAtWalkSpeed && lastJumpRushTime > jumpRushFrequency)
            {
                var addVelocityValue = (jumpWalkBonusToVelocity - 1) * ridbody.velocity.magnitude;
                var target = Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation) * Vector3.forward;
                var force = addVelocityValue * target.normalized;
                var addForceValue = ridbody.mass * force / Time.fixedDeltaTime;
                ridbody.AddForce(addForceValue, ForceMode.Force);
                lastJumpRushTime = 0;
            }
        }

        if (!jump && (isGrounded || isTouchingSlope || isInWater) && ridbody.velocity.y <= 0)
        {
            if(!isJumpFrequency)
                lastJumpLandTime = 0f;

            isJumpFrequency = true;
            if (hasJump)
            {
                canSlip = true;
                var eventObjectPrefab = Resources.Load<GameObject>("Prefabs/Effect/Puff");
                var eventObjectGameObject = Instantiate(eventObjectPrefab, new Vector3(this.transform.position.x, this.transform.position.y - 1.5f, this.transform.position.z), Quaternion.Euler(new Vector3(0, 0, 0)));
                hasJump = false;
            }
        }

    }

    public bool CanPick()
    {
        return true;
    }

    public void AddForce(Vector3 force,ForceMode forceMode, bool isForce = false)
    {
        if (!invulernable || isForce)
            ridbody.AddForce(force,forceMode);
    }

    public void AddExplosionForce(float explosionForce,Vector3 explosionPositon,float explosionRadius)
    {
            //{
            //    var target = (transform.position - explosionPositon).normalized;
            //    ridbody.AddForce(explosionForce * ( 1 / Mathf.Sqrt((explosionPositon - transform.position).magnitude)) * target,ForceMode.Force);
            //}
        if (!invulernable)
            //ridbody.AddExplosionForce(explosionForce * (1 / Mathf.Sqrt((explosionPositon - transform.position).magnitude)),explosionPositon,-1,2f);
            ridbody.AddExplosionForce(explosionForce * (1 / Mathf.Sqrt((explosionPositon - transform.position).magnitude)), explosionPositon,-1,(explosionPositon.y - (transform.position.y - (bodyCollider as SphereCollider).radius))+4f);
        //Debug.Log(explosionForce * (1 / Mathf.Sqrt((explosionPositon - transform.position).magnitude)));
        //Debug.Log($"{(explosionPositon.y - (transform.position.y - (bodyCollider as SphereCollider).radius)) + 4f} ... {explosionPositon} ... {transform.position} .. {(explosionPositon - transform.position).magnitude}");
    }

    private void UseItem(bool isUse)
    {
        if (isUse && itemAbility != null && !hasStunBuff() && !isBrake && !isBrakeRecover())
        {
            itemAbility.UseItemAbility();
        }
    }

    private void MoveCharge(bool charge)
    {
        if (isDead || hasStunBuff() || controlKeepRuning)
            return;
        if (charge)
        {
            canSlip = false;
            if (!releasing)
            {
                elapsedChargeTime = 0;
            }
            else
            {
                elapsedChargeTime += Time.fixedDeltaTime;
            }
            currentStamina = currentStamina - maxActorStamina / chargeTime * Time.fixedDeltaTime;
            currentStamina = Mathf.Max(0, currentStamina);
            if (currentStamina > minStaminaThreshold)
            {
                // currentGas = currentGas + (maxActorGas - currentGas) / chargeTime * Time.fixedDeltaTime;
                releasing = true;

                lastSpeedUpTime = 0;
            }
            if (currentStamina <= 0)
            {
                releasing = false;
                if (ridbody.velocity.magnitude >= runMaxVelocity * 0.85f && (isTouchingSlope || isGrounded))
                {
                    var buff = new SliperyBuff(this);
                    OnGainBuff(buff);
                    anima.SetBool("isBrake", true);
                }
                //fullyRecoveringStamina = true;
            }
        }
        else
        {
            if (ridbody.velocity.magnitude >= runMaxVelocity * 0.85f && (isTouchingSlope || isGrounded) && releasing)
                canSlip = true;
            releasing = false;
        }
    }

    private void SetGPHint()
    {
        if (fullyRecoveringStamina)
        {
            Color color = Color.Lerp(staminaRecoveringColorRed, staminaRecoveringColorYellow, (Mathf.Sin(4*Time.time) + 1) / 2);
            staminaFillImage.color = color;
        }
        else
        {
            if (staminaFillImage.color != staminaNormalColor)
            {
                staminaFillImage.color = staminaNormalColor;
            }
        }
    }

    private void SetHPHint()
    {
        if (isDead)
        {
            //var rendererBlock = new MaterialPropertyBlock();
            //skinnedMeshRenderer.GetPropertyBlock(rendererBlock, 1);
            //if (rendererBlock.GetFloat("_TintAmount") != 0)
            //{
            //    rendererBlock.SetFloat("_TintAmount", 0f);
            //    skinnedMeshRenderer.SetPropertyBlock(rendererBlock, 1);
            //}
            if (xfurInstance.FurDataProfiles[1].FurEmissionColor != normalHPColor)
            {
                xfurInstance.FurDataProfiles[1].FurEmissionColor = normalHPColor;
            }
        }
        else if (invulernable)
        {
            if (xfurInstance.FurDataProfiles[1].FurEmissionColor != invulenableHPColor)
            {
                xfurInstance.FurDataProfiles[1].FurEmissionColor = invulenableHPColor;
            }
        }
        else
        {
            if (currentHPValue < dangerHpTip)
            {
                //var rendererBlock = new MaterialPropertyBlock();
                //skinnedMeshRenderer.GetPropertyBlock(rendererBlock, 1);
                //rendererBlock.SetFloat("_TintAmount", blinkCurve.Evaluate(Time.time));
                //skinnedMeshRenderer.SetPropertyBlock(rendererBlock, 1);
                xfurInstance.FurDataProfiles[1].FurEmissionColor = Color.Lerp(normalHPColor, dangerHPColor, blinkCurve.Evaluate(Time.time));
            }
            else
            {
                //var rendererBlock = new MaterialPropertyBlock();
                //skinnedMeshRenderer.GetPropertyBlock(rendererBlock, 1);
                //if (rendererBlock.GetFloat("_TintAmount") != 0)
                //{
                //    rendererBlock.SetFloat("_TintAmount", 0f);
                //    skinnedMeshRenderer.SetPropertyBlock(rendererBlock, 1);
                //}
                if (xfurInstance.FurDataProfiles[1].FurEmissionColor != normalHPColor)
                {
                    xfurInstance.FurDataProfiles[1].FurEmissionColor = normalHPColor;
                }
            }
        }

    }

    public void SetFurColor(int index)
    {
        xfurInstance.FurDataProfiles[1].FurMainTint = furData.furDataList[index].furColor;
        xfurInstance.FurDataProfiles[1].FurRim = furData.furDataList[index].furColor;
        furIndex = index;
    }

    public void SetColor(float H, float S)
    {
        var rendererBlock = new MaterialPropertyBlock();
        skinnedMeshRenderer.GetPropertyBlock(rendererBlock, 1);
        rendererBlock.SetFloat("_H", H);
        rendererBlock.SetFloat("_S", S);
        skinnedMeshRenderer.SetPropertyBlock(rendererBlock, 1);
    }

    public void SetSnowCollider()
    {
        foreach(var collider in snowDectors)
        {
            collider.SetActive(!isInRocket);
        }
    }

    //private void MoveRoll()
    //{
    //    if (isDead)
    //        return;
    //    if (isStun)
    //    {
    //        if(ridbody.velocity.magnitude > stunStopRollMinVelocity)
    //        {
    //            rollRotationAxis = - Vector3.Cross(groundNormal, ridbody.velocity);
    //            rollRotationAmount = ridbody.velocity.magnitude;
    //            IKObject.transform.Rotate(rollRotationAxis, - rollRotationAmount, Space.World);
    //            isRollContinu = true;
    //        }
    //        else
    //        {
    //            var upAngle = Vector3.Angle(IKObject.transform.up, groundNormal);
    //            var forwardAngle = Vector3.Angle(IKObject.transform.forward, groundNormal);
    //            isRollContinu = upAngle < stunStopRollMinAngle || (180f - upAngle) < stunStopRollMinAngle || (stunStopRollMaxAngle < upAngle && upAngle < (180 - stunStopRollMaxAngle))&&(stunStopRollMaxAngle < forwardAngle && forwardAngle < (180 - stunStopRollMaxAngle));
    //            //近乎停止旋转时的平衡补偿
    //            if(isRollContinu)
    //            {
    //                IKObject.transform.Rotate(rollRotationAxis, -rollRotationAmount, Space.World);

    //            }
    //        }
    //    }
    //}

    #endregion

    #region Check

    private void CheckSlipery()
    {
        if (canSlip)
            lastSlipReadyTime += Time.fixedDeltaTime;
        else
            lastSlipReadyTime = 0;
        if (canSlip && lastSlipReadyTime >= slipProtectTime)
        {
            if (ridbody.velocity.magnitude >= runMaxVelocity * 0.85f && (isTouchingSlope || isGrounded))
            {
                canSlip = false;
                anima.SetBool("isBrake", true);
                var buff = new SliperyBuff(this);
                OnGainBuff(buff);
            }
        }
    }


    public void OnGainBuff(Buff buff)
    {
        buffs.Add(buff);
        buff.OnBuffApply();
        SetBuffControlState();
    }

    public void SetBuffControlState()
    {
        if (buffs.Any(x => x is RocketThrusterBuff))
        {
            releasing = false;

            controlKeepRuning = true;
        }
        else
        {
            controlKeepRuning = false;

        }
    }

    private bool HasRollStun()
    {
        return buffs.Any(x => x is QTERollStun);
    }

    public bool HasDamageImmuneBuff()
    {
        return buffs.Any(x => x is DamageImmuneBuff);
    }

    public void FinishUFOImmuneBuff()
    {
        foreach(var buff in buffs)
        {
            if (buff is UFODamageImmuneBuff)
            {
                buff.Finish();
            }
        }
    }

    public bool HasQTEStun()
    {
        return buffs.Any(x => x is QTEBuff);
    }

    public bool HasCollisionInEffectiveBuff()
    {
        return buffs.Any(x => x is CollisionIneffectiveBuff);
    }

    private void CheckStun()
    {
        if (isDead || HasRollStun())
            return;
        if (currentStunValue <= 0)
        {
            var QTERollStun = new QTERollStun(this, stunRecoverTime, 0.05f);
            OnGainBuff(QTERollStun);
        }
        //if (isStun)
        //{
        //    lastStunTime += Time.fixedDeltaTime;
        //    if (lastStunTime >= stunRecoverTime && !isRecoveringFromStun)
        //    {
        //        isRecoveringFromStun = true;
        //        stunRecoverTime *= stunAccumulateTime;
        //        //maxStunValue = Math.Max(stunMinValue, maxStunValue - stunDecreaseRate);
        //        currentStunValue = maxStunValue;

        //        IKObject.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f).onComplete += () =>
        //        {
        //            //stunProgress.gameObject.SetActive(false);
        //            stun.gameObject.SetActive(false);
        //            isStun = false;
        //            isRecoveringFromStun = false;
        //        };
        //    }
        //}
        //else
        //{
        //    if (currentStunValue <= 0)
        //    {
        //        //stunProgress.gameObject.SetActive(true);
        //        stun.gameObject.SetActive(true);
        //        isStun = true;
        //        lastStunTime = 0;
        //    }
        //}
    }

 
    private void CheckSpeed()
    {
        //Debug.LogError($"current speed: {velocityBeforeCollision.magnitude} ---> {(velocityBeforeCollision.magnitude / runMaxVelocity) * 100}% {(DateTime.Now - releaseDateTime).TotalSeconds} seconds");
        if (velocityBeforeCollision.magnitude >= runMaxVelocity * 0.9f && !isInRocket)
        {
            isAtMaxSpeed = true;
            anima.SetBool("isAtMaxSpeed", isAtMaxSpeed);
            if ((isGrounded || isTouchingSlope) && releasing)
                particle.Play();
        }
        else
        {
            isAtMaxSpeed = false;
            anima.SetBool("isAtMaxSpeed", isAtMaxSpeed);
            particle.Stop();
        }

        if (velocityBeforeCollision.magnitude >= movementMaxVelocity * 0.9f)
        {
            isAtWalkSpeed = true;
        }
        else
        {
            isAtWalkSpeed = false;
        }
        if (!isGrounded && !isTouchingSlope && !HasRollStun())
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

    private void SetView()
    {
        if (viewRangeDector.closeTargets.Count > 0 && viewRangeDector.closeTargets.FirstOrDefault(x => !x.isDead) != null)
        {
            var closeTarget = viewRangeDector.closeTargets.FirstOrDefault(x => !x.isDead);

            if (lastSeenView != null && gameController.screenMode == ScreenMode.Split)
            {
                cinemachineTargetGroup.RemoveMember(lastSeenView.transform);
            }

            if (gameController.screenMode == ScreenMode.Split && cinemachineTargetGroup.FindMember(closeTarget.transform) == -1)
            {
                cinemachineTargetGroup.AddMember(closeTarget.transform, 2, 2);
            }

            lastSeenView = viewRangeDector.closeTargets.FirstOrDefault(x => !x.isDead);


        }
        else
        {
            if (lastSeenView != null && gameController.screenMode == ScreenMode.Split)
            {
                cinemachineTargetGroup.RemoveMember(lastSeenView.transform);
            }
            lastSeenView = null;
        }

    }

    private void SetIK()
    {
        if (isDead)
        {
            grounderQuadruped.weight = 0;
            lookAtIK.solver.IKPositionWeight = 0;
            return;
        }
        var targetIK = buffs.Any(x => (x is QTEBuff qte && qte.requireIKOff)) ? 0 : 1;

        var targetAIMIK = 0;

        if (rangeDector.closeTargets.Count > 0 && rangeDector.closeTargets.FirstOrDefault( x => !x.isDead ) != null)
        {
            var target = rangeDector.closeTargets.FirstOrDefault(x => !x.isDead);
            var hitOnPlane = Vector3.ProjectOnPlane((target.transform.position - ridbody.position), groundNormal).normalized;
            var forwardOnPlane = Vector3.ProjectOnPlane(ridbody.transform.forward, groundNormal).normalized;
            var seenAngle = Vector3.SignedAngle(forwardOnPlane, hitOnPlane, groundNormal);
            var closeTarget = rangeDector.closeTargets.FirstOrDefault(x => !x.isDead);

            if (gameController.screenMode == ScreenMode.Split && cinemachineTargetGroup.FindMember(closeTarget.transform) == -1) {
                cinemachineTargetGroup.AddMember(closeTarget.transform, 2, 4);
            }

            if (Mathf.Abs(seenAngle) <= 90)
                lastSeenTarget = rangeDector.closeTargets.FirstOrDefault(x => !x.isDead);
            else
                lastSeenTarget = null;

        }
        else
        {
            if (lastSeenTarget != null && gameController.screenMode == ScreenMode.Split)
            {
                cinemachineTargetGroup.RemoveMember(lastSeenTarget.transform);
            }
            lastSeenTarget = null;
        }

        if (lastSeenTarget != null)
        {
            lookAtIK.solver.target = lastSeenTarget.transform;
            targetAIMIK = 1;
        }

        if (lastSeenTarget == null)
        {
            lookAtIK.solver.target = null;
            targetAIMIK = 0;
        }

        if (lookAtIK.solver.IKPositionWeight != targetAIMIK)
        {
            var speed = Time.deltaTime;
            if (targetAIMIK > lookAtIK.solver.IKPositionWeight)
            {
                lookAtIK.solver.IKPositionWeight += speed;
                lookAtIK.solver.IKPositionWeight = Mathf.Min(1, lookAtIK.solver.IKPositionWeight);
            }
            else
            {
                lookAtIK.solver.IKPositionWeight -= speed;
                lookAtIK.solver.IKPositionWeight = Mathf.Max(0, lookAtIK.solver.IKPositionWeight);
            }
        }


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
        if (isDead)
            return;
        if (floatObj.InWater && !isInWater)
        {
            //入水判定
            lastInWaterTime = 0;
        }

        isInWater = floatObj.InWater && !isInRocket;

        if (isInWater)
        {
            lastInWaterTime += Time.fixedDeltaTime;
        }

    }

    private void CheckIsGrounded()
    {
        isGrounded = Physics.CheckSphere(bodyCollider.transform.position + (bodyCollider as SphereCollider).center - new Vector3(0, (bodyCollider as SphereCollider).radius * bodyCollider.transform.localScale.y, 0), 0.02f, groundMask);
        anima.SetBool("OnGround", isGrounded || isTouchingSlope);
    }

    private void CheckSlopeAndDirections()
    {
        RaycastHit slopeHit;
        isTouchingSlope = false;
        if (Physics.SphereCast(bodyCollider.transform.position + (bodyCollider as SphereCollider).center + transform.forward.normalized * slopeCheckerForwardOffset, slopeCheckerThrashold, Vector3.down, out slopeHit, (bodyCollider as SphereCollider).radius * bodyCollider.transform.localScale.y + 0.01f, groundMask))
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

    #endregion
    #region SetUI

    private void SetAnimState()
    {
        anima.SetBool("isWalk", isWalk);
    }

    private void SetQTE()
    {
        if (!gameController.debug)
        {
            stunSlider.gameObject.SetActive(false);
            stunProgress.gameObject.SetActive(false);
        }
        else
        {
            stunProgress.gameObject.SetActive(true);
            stunSlider.gameObject.SetActive(true);
        }

        //当前QTE
        var currentQTE = this.buffs.Where(x => x is QTEBuff).Cast<QTEBuff>().ToList();
        var chosenOne = currentQTE.FirstOrDefault(x => x.QTEPriority == -1);
        if (chosenOne == null && currentQTE.Count > 0)
        {
            var maxP = currentQTE.Max(x => x.QTEPriority);
            chosenOne = currentQTE.FirstOrDefault(x => x.QTEPriority == maxP);
        }

        if (chosenOne != null)
        {
            stunProgress.fillAmount = chosenOne.buffTime / chosenOne.maxBuffTime;
            if (!stunButtonHint.activeSelf)
            {
                stunButtonHint.SetActive(true);

                gpSlider.gameObject.SetActive(false);
            }
        }
        else
        {
            stunSlider.value = (float)(currentStunValue / maxStunValue);
            if (!gpSlider.gameObject.activeSelf)
            {
                stunButtonHint.SetActive(false);

                gpSlider.gameObject.SetActive(true);
            }
        }
        stunSlider.transform.position = this.transform.position;
        stunSlider.transform.localPosition = stunSlider.transform.localPosition + new Vector3(0, 1.5f + (this.transform.localScale.x - 1) * 1.2f, 0);

        stunProgress.transform.position = this.transform.position;
        stunProgress.transform.localPosition = stunProgress.transform.localPosition + new Vector3(-1f, 1.5f + (this.transform.localScale.x - 1) * 1.2f, 0);
        stunButtonHint.transform.position = this.transform.position;
        stunButtonHint.transform.localPosition = stunButtonHint.transform.localPosition + new Vector3(0, 1.25f + (this.transform.localScale.x - 1) * 1.2f, 0);

    }

    private void SetSlider()
    {
        if (!gameController.debug)
        {
            hpSlider.gameObject.SetActive(false);
        }
        else
        {
            hpSlider.gameObject.SetActive(true);
        }
        gpSlider.value = (float)(currentStamina / maxActorStamina);

        hpSlider.value = (float)(currentHPValue / maxHPValue);
        if (gameController.screenMode == ScreenMode.Same)
            canvas.transform.forward = gameController.mainCamera.transform.forward;
        else if (gameController.screenMode == ScreenMode.Split)
            canvas.transform.forward = m_camera.transform.forward;

        //临时缩放
        canvas.transform.localScale = Vector3.one / transform.localScale.x;
        gpSlider.transform.position = this.transform.position;
        ////stunSlider.transform.position = this.transform.position;
        hpSlider.transform.position = this.transform.position;
        gpSlider.transform.localPosition = gpSlider.transform.localPosition + new Vector3(0, 1.25f + (this.transform.localScale.x - 1) * 1.2f, 0);
        //stunSlider.transform.localPosition = stunSlider.transform.localPosition + new Vector3(0, 1.5f + (this.transform.localScale.x - 1) * 1.2f, 0);
        hpSlider.transform.localPosition = hpSlider.transform.localPosition + new Vector3(0, 1.75f + (this.transform.localScale.x - 1) * 1.2f, 0);
        playerIndicator.transform.position = this.transform.position;
        playerIndicator.transform.localPosition = playerIndicator.transform.localPosition + new Vector3(0, 1.75f + (this.transform.localScale.x - 1) * 1.5f, 0);


        //stunProgress.fillAmount = lastStunTime / stunRecoverTime;
        //stunProgress.transform.position = this.transform.position;
        //stunProgress.transform.localPosition = stunProgress.transform.localPosition + new Vector3(-1f, 1.5f + (this.transform.localScale.x - 1) * 1.2f, 0);

    //    if (isStun)
    //    {
    //        if (!stunButtonHint.activeSelf)
    //        {
    //            stunButtonHint.SetActive(true);

    //            gpSlider.gameObject.SetActive(false);
    //        }
    //        stunButtonHint.transform.position = this.transform.position;
    //        stunButtonHint.transform.localPosition = stunButtonHint.transform.localPosition + new Vector3(0, 1.25f + (this.transform.localScale.x - 1) * 1.2f, 0);
    //    }
    //    else
    //    {
    //        if (!gpSlider.gameObject.activeSelf)
    //        {
    //            stunButtonHint.SetActive(false);

    //            gpSlider.gameObject.SetActive(true);
    //        }
    //    }
    }
    private void SetRingColor()
    {
        //var rendererBlock = new MaterialPropertyBlock();
        //ringRenderer.GetPropertyBlock(rendererBlock, 0);
        //rendererBlock.SetColor("_Color", InputReadManager.Instance.playerColors[playerIndex]);
        //ringRenderer.SetPropertyBlock(rendererBlock, 0);
        playerIndexText.text = $"P{playerIndex}";
        if (InputReadManager.Instance.playerIndicatorSprites.Count >= playerIndex && playerIndex > 0)
            playerIndicator.GetComponent<Image>().sprite = InputReadManager.Instance.playerIndicatorSprites[playerIndex - 1];
    }

    // temp, 告诉用户到达最大速
    private void SetRingMaxColor()
    {
        //var rendererBlock = new MaterialPropertyBlock();
        //ringRenderer.GetPropertyBlock(rendererBlock, 0);
        //rendererBlock.SetColor("_Color", Color.red);
        //ringRenderer.SetPropertyBlock(rendererBlock, 0);
    }


    #endregion


    #region OnCollison
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
            var d2 = Vector3.Angle(vel2, contactToOther);

            var degree1 = d1 * Mathf.Deg2Rad;
            var degree2 = d2 * Mathf.Deg2Rad;

            Vector3 impactVelocity = collision.relativeVelocity;

            var m1 = (Mathf.Cos(degree1) * vel1).magnitude; //我针对对方的力
            var m2 = (Mathf.Cos(degree2) * vel2).magnitude; //对方针对我的力

            var m = m1 + m2;

            //var hitDir = Vector3.ProjectOnPlane((ridbody.position - collision.contacts[0].point), Vector3.up).normalized;
            //var force = KnockBackForce(0.5f, hitDir);


            //if (otherCollision.releasing)
            //    ridbody.AddForce((force.force) * hitDir, ForceMode.Force);

        }

       
    }

    private void OnCollisionExit(Collision collision)
    {
        lastCollisionTime = 0;
        isCollidingCharacter = false;
    }

    private struct KnockBackForceStruct
    {
        public float force;
        public float hitTime;
    }


    bool isRecordingHit = false;
    bool isFirstFrame = false;
    Vector3 hitPosition;
    DateTime hitTime;
    float hitDistance = 0;
    bool hasPrint = false;
    private void RecordCollisionVelocity()
    {
        if (isRecordingHit)
        {        
            if (isFirstFrame && !hasPrint)
            {
                //Debug.LogError($" ===> {this.gameObject.name} === > 第一帧 实际距离：{(this.transform.position - hitPosition).magnitude} 当前速度:{this.ridbody.velocity.magnitude} 目标距离：{hitDistance} 用时: {((DateTime.Now - hitTime).TotalSeconds)}");
                isFirstFrame = false;
                hasPrint = true;
            }
            isFirstFrame = true;
            if ((DateTime.Now - hitTime).TotalSeconds > 0.2f && this.ridbody.velocity.magnitude < 0.1f)
            {
                //Debug.LogError($" ===> {this.gameObject.name} === > 实际距离：{(this.transform.position - hitPosition).magnitude} 当前速度:{this.ridbody.velocity.magnitude} 目标距离：{hitDistance} 用时: {((DateTime.Now - hitTime).TotalSeconds)}");
                isRecordingHit = false;
            }
        }
    }

    /// <summary>
    /// 结算击退距离
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    private KnockBackForceStruct KnockBackForce(float distance, Vector3 hitDir)
    {
        isRecordingHit = true;
        hasPrint = false;
        hitDistance = distance;
        hitPosition = this.transform.position;
        hitTime = DateTime.Now;
        var gravityDivide = Vector3.ProjectOnPlane(Physics.gravity, groundNormal) * ridbody.mass;
        var gravityFrictionDivide = Physics.gravity - gravityDivide;
        var frictionForceAcceleration =  bodyCollider.material.dynamicFriction * gravityFrictionDivide.magnitude;


        //var deltaV = Vector3.ProjectOnPlane((this.ridbody.velocity - velocityBeforeCollision), groundNormal);
        var deltaV = Vector3.ProjectOnPlane(this.ridbody.velocity, groundNormal);
        var magnitudeDeltaV = Vector3.Dot(deltaV, hitDir.normalized);

        var desiredV = (float)Math.Sqrt((2 * frictionForceAcceleration * distance));

        var desiredV0 = desiredV - magnitudeDeltaV;
        var acceleration = desiredV0 / Time.fixedDeltaTime;
        //Debug.LogError($"====> {this.gameObject.name} === > 距离:{distance} 当前速度:{this.ridbody.velocity.magnitude} 理论VO：{desiredV0}");
        return new KnockBackForceStruct {
            force = acceleration * ridbody.mass,
            hitTime = Mathf.Abs(desiredV / frictionForceAcceleration)
        };
    }

    /// <summary>
    /// 在空中的击退距离
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    private KnockBackForceStruct KnockBackOnAirForce(float distance, Vector3 hitDir)
    {
        isRecordingHit = true;
        hasPrint = false;
        hitDistance = distance;
        hitPosition = this.transform.position;
        this.hitTime = DateTime.Now;
        var currentVelocity = ridbody.velocity;
        float forceMagnitude = 0f;
        float hitTime = 0f;
        var gravityDivide = Vector3.ProjectOnPlane(Physics.gravity, groundNormal) * ridbody.mass;
        var gravityFrictionDivide = Physics.gravity - gravityDivide;
        var frictionForceAcceleration = bodyCollider.material.dynamicFriction * gravityFrictionDivide.magnitude;
        var deltaV = Vector3.ProjectOnPlane(this.ridbody.velocity, groundNormal);
        var magnitudeDeltaV = Vector3.Dot(deltaV, hitDir.normalized);

        //var deltaV = (this.ridbody.velocity - velocityBeforeCollision);
        //var magnitudeDeltaV = Vector3.Dot(deltaV, hitDir.normalized);
        RaycastHit hit;
        if (Physics.Raycast(bodyCollider.transform.position + (bodyCollider as SphereCollider).center - new Vector3(0, (bodyCollider as SphereCollider).radius, 0), -transform.up, out hit, 10, groundMask)) 
        {
            if (currentVelocity.y > 0)
            {
                //上升状态处理
                var riseTime = currentVelocity.y / currentGravity.magnitude;
                var dropHeight = hit.distance + currentGravity.magnitude * Mathf.Pow(riseTime, 2.0f)/2;
                var dropTime = Mathf.Sqrt(2 * dropHeight / currentGravity.magnitude);


                var initialVelocity =Mathf.Sqrt(frictionForceAcceleration*(frictionForceAcceleration * (Mathf.Pow (dropTime + riseTime,2))+ 2 * distance)) -(dropTime + riseTime)* frictionForceAcceleration;
                var initialVelocityDelta = initialVelocity - magnitudeDeltaV;

                var acceleration = initialVelocityDelta / Time.fixedDeltaTime;
                hitTime = riseTime + dropTime + Mathf.Abs(initialVelocity / frictionForceAcceleration);
                forceMagnitude = acceleration * ridbody.mass;
                //Debug.LogError($"====>上升 {this.gameObject.name} === > 距离:{distance} 当前速度:{this.ridbody.velocity.magnitude} 理论VO：{initialVelocity} 实际施加速度 {initialVelocityDelta}");
            }
            else
            {
                //下降状态处理
                var riseTime = Mathf.Abs(currentVelocity.y) / currentGravity.magnitude;
                var dropHeight = hit.distance + currentGravity.magnitude * Mathf.Pow(riseTime, 2.0f)/2;
                var dropMaxTime = Mathf.Sqrt(2 * dropHeight / currentGravity.magnitude);
                var dropTime = dropMaxTime - riseTime;
                var initialVelocity = Mathf.Sqrt(frictionForceAcceleration * (frictionForceAcceleration * (Mathf.Pow(dropTime, 2)) + 2 * distance)) - (dropTime) * frictionForceAcceleration;
                var initialVelocityDelta = initialVelocity - magnitudeDeltaV;


                hitTime = dropTime + Mathf.Abs(initialVelocity / frictionForceAcceleration);
                forceMagnitude = initialVelocityDelta / Time.fixedDeltaTime * ridbody.mass;
                //Debug.LogError($"====>下降 {this.gameObject.name} === > 距离:{distance} 当前速度:{this.ridbody.velocity.magnitude} 理论VO：{initialVelocity} 实际施加速度 {initialVelocityDelta}");
            }
        };

        return new KnockBackForceStruct { force = forceMagnitude, hitTime = hitTime };
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var buff in buffs)
        {
            buff.OnCollide(collision);
        }

        //撞击场景物体处理
        if (collision.gameObject.GetComponent<InteractiveObject>())
        {

            //自身速度
            Vector3 velocitySelf = new Vector3(velocityBeforeCollision.x, velocityBeforeCollision.y, velocityBeforeCollision.z);
            velocitySelf = Vector3.ProjectOnPlane(velocitySelf, groundNormal).normalized * velocitySelf.magnitude;


            Vector3 cPoint = collision.contacts[0].point;
            Vector3 contactToMe = cPoint - positionBeforeCollision;

            var angleSelf = Vector3.Angle(velocitySelf, contactToMe);
            var degreeSelf = angleSelf * Mathf.Deg2Rad;


            var momentumSelf = (Mathf.Cos(degreeSelf) * velocitySelf).magnitude;
            var myBuff = isAtMaxSpeed && !isGrounded ? buffAttack : 1;


            var myHitKnockbackCoef = speedToInteractiveEffect.Evaluate(momentumSelf * myBuff);

            if (myHitKnockbackCoef < 0.1f)
                return;

            var eventObjectPrefab = Resources.Load<GameObject>("Prefabs/Effect/StunHit");
            var eventObjectGameObject = Instantiate(eventObjectPrefab, collision.contacts[0].point, Quaternion.Euler(new Vector3(0, 0, 0)));
            var hitDir = Vector3.ProjectOnPlane((ridbody.position - collision.contacts[0].point), Vector3.up).normalized;
            var targetDistance = Math.Min(hitMaxDistance, collision.gameObject.GetComponent<InteractiveObject>().knockbackDistance * myHitKnockbackCoef + hitKnockbackSelfCurve.Evaluate(momentumSelf * myBuff + 2)) * InteractiveDistanceCoef * hitKnockbackToSelfArgument;
            var forceData = KnockBackForce(targetDistance, hitDir);
            var targetStun = targetDistance * distanceToStunCoef * InteractiveStunDistanceCoef;
            if (collision.gameObject.GetComponent<InteractiveObject>().canStun)
                TakeStun((int)(targetStun));
            var buff = new HitBuff(this, forceData.hitTime);
            ridbody.AddForce((forceData.force) * hitDir, ForceMode.Force);
            this.OnGainBuff(buff);
            var hitOnPlane = Vector3.ProjectOnPlane((collision.contacts[0].point - ridbody.position), groundNormal).normalized;
            var forwardOnPlane = Vector3.ProjectOnPlane(ridbody.transform.forward, groundNormal).normalized;
            var hitAngle = Vector3.SignedAngle(forwardOnPlane, hitOnPlane, groundNormal);
            anima.SetFloat("hitAngle", hitAngle);
            anima.SetBool("isHit", true);
            



        }
        //撞击角色处理
        if (collision.gameObject.GetComponent<CharacterContorl>())
        {

            isCollidingCharacter = true;
            //特效
            var eventObjectPrefab = Resources.Load<GameObject>("MediumHit");
            var eventObjectGameObject = Instantiate(eventObjectPrefab, collision.contacts[0].point, Quaternion.Euler(new Vector3(0, 0, 0)));

            var otherCollision = collision.gameObject.GetComponent<CharacterContorl>();

            if (otherCollision.HasCollisionInEffectiveBuff())
                return;

            //自身速度
            Vector3 velocitySelf = new Vector3(velocityBeforeCollision.x, velocityBeforeCollision.y, velocityBeforeCollision.z);
            velocitySelf = Vector3.ProjectOnPlane(velocitySelf, groundNormal).normalized * velocitySelf.magnitude;
            //对方速度
            Vector3 velocityOther = new Vector3(otherCollision.velocityBeforeCollision.x, otherCollision.velocityBeforeCollision.y, otherCollision.velocityBeforeCollision.z);
            velocityOther = Vector3.ProjectOnPlane(velocityOther, otherCollision.groundNormal).normalized * velocityOther.magnitude;

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

            if (angleOther > 90)
                momentumOther = 0;

            Debug.Log($"{transform.name} velocitySelf:{velocitySelf} velocityOther:{velocityOther} angleSelf:{angleSelf} angleOther:{angleOther} momentumSelf:{momentumSelf}  momentumOther:{momentumOther}");

            //出招加成
            var hasBuff = (otherCollision.isAtMaxSpeed && (!otherCollision.isGrounded && !otherCollision.isTouchingSlope)) ? buffAttack : 1;
            var myBuff = isAtMaxSpeed && !isGrounded ? buffAttack : 1;

            var hitDir = Vector3.ProjectOnPlane((ridbody.position - collision.contacts[0].point), groundNormal).normalized;
            var myHitKnockback = hitKnockbackCurve.Evaluate(momentumSelf * myBuff);
            var otherHitKnockback = hitKnockbackCurve.Evaluate(momentumOther * hasBuff);

            KnockBackForceStruct forceData;
            //打击角度计算
            var hitOnPlane = Vector3.ProjectOnPlane((collision.contacts[0].point - ridbody.position), groundNormal).normalized;
            var forwardOnPlane = Vector3.ProjectOnPlane(ridbody.transform.forward, groundNormal).normalized;
            var hitAngle = Vector3.SignedAngle(forwardOnPlane, hitOnPlane, groundNormal);
            anima.SetFloat("hitAngle", hitAngle);

            if (Mathf.Abs(hitAngle) <= 45)
            {
                momentumOther = momentumOther * hitBonusToHead;
            }
            else if (Mathf.Abs(hitAngle) > 45 && Mathf.Abs(hitAngle) < 135)
            {
                momentumOther = momentumOther * hitBonusToSide;

            }
            else
            {
                momentumOther = momentumOther * hitBonusToBack;

            }

            var targetDistance = Math.Min(otherCollision.hitMaxDistance, hitKnockbackCurve.Evaluate(momentumOther * hasBuff) * otherCollision.hitKnockBackToOtherArgument + hitKnockbackSelfCurve.Evaluate(momentumSelf ))*hitKnockbackToSelfArgument;

            if (targetDistance < 0.1f)
                return;

            //施加水平推力
            if (isGrounded || isTouchingSlope)
                forceData = KnockBackForce(targetDistance, hitDir);
            else
                forceData = KnockBackOnAirForce(targetDistance, hitDir);
            //anima.SetBool("isHit", true);
            if (myHitKnockback > 1 || otherHitKnockback > 1)
            {
                anima.SetBool("isHit", true);
            }
            //Debug.LogError($"==> {this.gameObject.name} ===> 打击距离 {targetDistance} otherAtMaxSpeed? {otherCollision.isAtMaxSpeed} ");
            //打击眩晕和血量
            TakeStun((int)(hitKnockbackCurve.Evaluate(momentumOther * hasBuff * otherCollision.hitKnockBackToOtherArgument) * hitKnockbackToSelfArgument * distanceToStunCoef));
            TakeDamage((int)(hitKnockbackCurve.Evaluate(momentumOther * hasBuff * otherCollision.hitKnockBackToOtherArgument) * hitKnockbackToSelfArgument * distanceToHPCoef), hitDir);
            //TakeDamage(100, hitDir);//测试
            if (!invulernable)
            {
                var buff = new HitBuff(this, forceData.hitTime);
                ridbody.AddForce((forceData.force) * hitDir, ForceMode.Force);
                this.OnGainBuff(buff);
            }

            //施加转角力 正值顺时针转动，负值逆时针转动
            var torgueAngle = Vector3.SignedAngle(velocityOther, contactToOther, groundNormal);
            if (torgueAngle >= 0)
            {
                ridbody.AddRelativeTorque(Vector3.up * velocityOther.magnitude * Mathf.Cos(torgueAngle * Mathf.Deg2Rad)*50  , ForceMode.Force);
            }
            else
            {
                ridbody.AddRelativeTorque(Vector3.down * velocityOther.magnitude * Mathf.Cos(torgueAngle * Mathf.Deg2Rad)*50 , ForceMode.Force);
            }

            //Debug.LogError($"结算 {otherCollision.gameObject.name} force {force} hit Dir {hitDir}");


            ////旋转
            //if (isStun)
            //{
            //    var targetTime = hitKnockbackCurve.Evaluate(momentumOther * hasBuff);
            //    TargetRollTime = Math.Max(targetTime, TargetRollTime);
            //    if (TargetRollTime - ElapsedRollTime < targetTime)
            //    {
            //        TargetRollTime += (targetTime - (TargetRollTime - ElapsedRollTime));
            //    }
            //}
        }

    }
    #endregion

}
