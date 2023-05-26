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
    [Header("������")]
    public InputReaderBase inputReader;
    [Space(10)]
    [Header("��·����")]
    [Header("��·�ٶ�������Ҫʱ��")]
    public float movementSpeedUpTime;
    [Header("��·ת��Lerpϵ��")]
    public float movementRotationRate;
    [Header("��������ٶ�")]
    public float movementMaxVelocity;
    [Space(10)]
    [Header("��Ծ��ϵ��")]
    public float jumpForce;
    [Header("��Ծ���")]
    public float jumpFrequency = 1f;
    [Header("��Ծ�ӳ�")]
    public float jumpBonusToVelocity = 1.4f;

    [Space(10)]
    [Header("�ܲ�״̬��ز���")]
    [Header("�ܲ�����ٶ�")]
    public float runMaxVelocity;
    [Header("�ܲ��ٶ�������Ҫʱ��")]
    public float runSpeedUpTime;
    [Header("�ܲ��ٶ�����ʱת��Lerpϵ��")]
    public float runSpeedUpRotationRate;
    [Header("�ܲ�����ٶ�ʱת��Lerpϵ��")]
    public float runMaxSpeedRotationRate;
    [Header("ɲ��ʱ��ת��Lerpϵ��")]
    public float breakRotationRate = 0.02f;
    [Header("�ܲ�����ʱ��")]
    public float chargeTime;
    [Header("����ܲ�ʱ��")]
    public float releaseTime;

    [Space(10)]
    [Header("ɲ����ز���")]
    [Header("������ϵ��ϵ��")]
    public float decelerationForceArgument;
    [Header("����Ť����ϵ��ϵ��")]
    public float decelerationTorqueArgument;
    [Header("���ɲ��ʱ��")]
    public float maxBreakTime;
    [Header("��СƯ�ƽǶ�")]
    public float minDriftAngle = 30f;


    [Space(10)]
    [Header("б���������")]
    [Header("�����Layers")]
    [SerializeField] public LayerMask groundMask;
    [Header("б�¾���ײ��ļ�����")]
    public float slopeCheckerThrashold = 0.51f;
    [Header("б�¼������ǰƫ��")]
    public float slopeCheckerForwardOffset = 0.41f;
    [Header("��Сб�½Ƕ�")]
    public float miniClimbableSlopeAngle = 5;
    [Header("���б�½Ƕ�")]
    public float maxClimbableSlopeAngle = 40f;
    
    [Space(10)]
    [Header("ѣ�β��������")]
    [Header("ѣ�β����ֵ")]
    public float maxStunValue = 100;
    [Header("Ѫ�����ֵ")]
    private float maxHPValue = 100;
    [Header("ײ����ӳ������ֵ������")]
    public AnimationCurve forceToHuntCurve;
    [Header("����ָ�ʱ��")]
    public float cureTime;
    [Header("ѣ�λظ�ʱ��")]
    public float stunRecoverTime = 50;
    [Header("�ٶȻ�����ת�ٶȲ���")]
    public float velocityToRollAngleArgument = 1;
    [Header("ѣ����תֹͣʱ���Ƕ�")]
    public float stunStopRollMaxAngle;
    [Header("ѣ����תֹͣʱ��С�Ƕ�")]
    public float stunStopRollMinAngle;
    [Header("ѣ��ʱֹͣת������С�ٶ���ֵ")]
    public float stunStopRollMinVelocity;

    [Header("�Ƿ�ﵽ����ٶ�")]
    public bool isAtMaxSpeed = false;
    [Header("�����ٶ�����")]
    public AnimationCurve runAnimCurve;
    [Header("�����ٶȲ�������")]
    public AnimationCurve runAnimPlayCurve;
    [Header("�ٶȻ�������")]
    public AnimationCurve hitKnockbackCurve;
    [Header("�����ٶȻ���ȡ��")]
    public AnimationCurve hitKnockbackSelfCurve;
    [Header("���˷�Χ����")]
    public float hitMaxDistance = 5;

    [Header("�������")]
    [Header("��������")]
    public float gravityScale = 1;
    [Header("��׹��������")]
    public float fallingGravityScale = 1;
    public float ascendingGravityScale = 1;

    [Header("���мӳ�")]
    public float buffAttack = 2f;
    [Header("������뻻��ѣ��")]
    public float distanceToStunCoef = 10f;
    [Header("������뻻��Ѫ��")]
    public float distanceToHPCoef = 2.5f;

    [Space(10)]
    [Header("�����Ҫ�������")]
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
    public RagdollActivator ragdollController;
    public GameObject playerIndicator;
    public TMPro.TMP_Text playerIndexText;
    //������
    public float forceArgument;
    //������ϵ��
    [Range(0, 2)]
    public float receivedForceRate = 1;

    //���淨��
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
    //����
    public bool isRecovering { get; private set; }

    //ҡ������ֵ��Сֵ
    private float movementThrashold = 0.35f;
    public float sensitivity = 0.5f;
    private Vector3 initialRotation;
    private Vector3 forceTarget;
    private Quaternion initialRot;
    // ���ز���
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
    //��Ч��Դ
    //��ǰ���������С
    public Vector3 currentGravity;

    public List<Buff> buffs = new List<Buff>();

    public int playerIndex = 0;

    //ѣ����ת״̬���
    Vector3 rollRotationAxis = Vector3.zero;
    float rollRotationAmount = 0f;


    public bool isInWater = false;
    private bool hasInWater = false;

    //ɲ��ʱ�ĳ���
    private Vector3 initialBrakeTarget = Vector3.zero;


    //���ؼ�ʱ�����
    private float lastJumpLandTime;
    private float lastStunTime;
    private float ElapsedRollTime; //��ȥ�˵�ʱ��
    private float TargetRollTime; //Ŀ��ʱ��
    private float lastInWaterTime; //�ϴ���ˮʱ��
    private float lastCollisionTime; //�ϴ���ײʱ��
    private float lastHPSubtractTime = 0; //�ϴο�Ѫʱ��
    private float lastHPRecoveryTime = 0; //�ϴλ�Ѫʱ��
    private float lastStunRecoveryTime = 0; //�ϴ�ѣ�λظ�ʱ��

    //�Ƿ���ѣ��״̬
    //����ʱ��
    [HideInInspector]
    private float recoveryTime = 1;
    [Header("��Ѫ���ʱ��")]
    public float hpRecoveryFrequency = 1;
    [Header("��Ѫ��ʱʱ��")]
    public float damageRecoveryFrequency = 5;
    [Header("��Ѫ��")]
    public float HPRecoveryRate = 2;
    [Header("��Ѫ��")]
    public float HPSubtractRate = 5;
    [Header("��Ѫ���ʱ��")]
    public float hpSubtractFrequency = 1;
    [Header("��ˮ��Ѫ��ʱʱ��")]
    public float inWaterSubtractFrequency = 3;
    [Header("ѣ�λظ����ʱ��")]
    public float stunRecoveryFrequency = 1;
    [Header("ѣ�λظ���")]
    public float stunRecoveryRate = 2;
    [Header("ѣ�λ�Ѫ��ʱʱ��")]
    public float withoutCollidingRecoveryFrequency = 3;

    //�Ƿ�����ײ��ɫ
    private bool isCollidingCharacter = false;
    [Header("ѣ�����޼���")]
    public float stunDecreaseRate = 10;
    [Header("ѣ����Сֵ")]
    public float stunMinValue = 50;
    [Header("�����������˥��ϵ��")]
    [Range(0, 1)]
    public float InteractiveDistanceCoef = 1f;
    [Range(0, 1)]
    public float InteractiveStunDistanceCoef = 1f;
    [Header("�������巴���ٶȸ��ݽ�ɫײ���ٶ�")]
    public AnimationCurve speedToInteractiveEffect;

    public GameObject crown;

    [Header("����״̬��ʾѪ��")]
    public float dangerHpTip = 30f;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public AnimationCurve blinkCurve;

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
        crown.gameObject.SetActive(false);

        //impactEffect.gameObject.SetActive(false);
        smokeEffect.gameObject.SetActive(true);
        particle.Stop();
        crown.gameObject.SetActive(false);
        ragdollController.Ragdoll(false, Vector3.zero);
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
        CheckHP();
        //SetEffect();
    }

    private void Update()
    {
        SetAnimatorArgument();
        SetIK();
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            velocityBeforeCollision = GetComponent<Rigidbody>().velocity;
            positionBeforeCollision = GetComponent<Rigidbody>().position;
        }
           
        CheckStun();
        CheckSpeed();
        CheckInVulernable();
        CheckIsGrounded();
        UpdateBuff();
        CheckSlopeAndDirections();
        CheckIsInWater();
        UpdateHP();
        UpdateStunRecovery();
        SetGravity();

        RecordCollisionVelocity();

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

    private void SetDead(Vector3 hitDir)
    {
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
        inputReader.releaseAciton = null;
        inputReader.interactWeaponAction = null;
        inputReader.jumpAction = null;
        inputReader.brakeAciton = null;
        currentGas = 0;
        crown.transform.DOLocalRotate(new Vector3(crown.transform.localRotation.eulerAngles.x, 360, 0), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    public void TakeStun(int number)
    {
        if (isStun)
            return;
        currentStunValue = Math.Max(0, currentStunValue - number);
        CheckStun();
    }

    public void TakeDamage(float number, Vector3 hitDir)
    {
        currentHPValue -= number;
        currentHPValue = Math.Max(currentHPValue, 0);
        lastHPSubtractTime = 0;
        if (currentHPValue == 0)
        {
            SetDead(hitDir);
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

    private void UpdateHP()
    {
        if (gameController.isGameOver)
            return;
        if (isDead)
            return;

        if (isInWater)
        {
            //��ˮ��Ѫ�ж�
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
        anima.SetBool("Releasing", releasing);
        anima.SetBool("isStun", isStun);
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

    IEnumerator DelayRemove()
    {
        yield return new WaitForSeconds(3);
        var cinemachineTargetGroup = GameObject.FindObjectOfType<CinemachineTargetGroup>();
        cinemachineTargetGroup.RemoveMember(transform);
    }

    private void Dead()
    {
        StartCoroutine(DelayRemove());
        //this.gameObject.SetActive(false);
        this.canvas.gameObject.SetActive(false);
        gameController.CheckGameState();
    }

    private void SetGravity()
    {
        if (isDead)
            return;
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
        if (isStun || isDead)
            return;
        //��λ�����뷽��
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

                //������������
                var moveTarget = ridbody.transform.forward;
                moveTarget = moveTarget.normalized;
                moveTarget = Vector3.ProjectOnPlane(moveTarget, groundNormal).normalized;
                if (!buffs.Any(x => x is HitBuff))
                    ridbody.AddForce(moveTarget * forceMagnitude - gravityDivide, ForceMode.Force);
                //Debug.Log($"isTouchingSlope || isGrounded {isTouchingSlope || isGrounded} forceMagnitude {forceMagnitude} velocity {ridbody.velocity} velocityMagnitude {ridbody.velocity.magnitude}");
                CheckisDrift();



                if (axisInput.magnitude > movementThrashold)
                {
                    targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                    if(hasBrake)
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
                    targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                    if(hasBrake)
                        transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), breakRotationRate);
                    else
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
                
                //���ɲ����·״̬����ʩ���ƽ���
                if(!hasBrake)
                {
                    if (!buffs.Any(x => x is HitBuff))
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
        if (isDead)
            return;
        if (isStun || isInWater)
        {
            if (hasBrake)
            {
                anima.SetBool("isBrake", false);
                hasBrake = false;
            }
            return;
        }
        //ֻ�е�����ɲ��
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
        if ((isGrounded || isTouchingSlope) && ridbody.velocity.y <= 0)
        {
            anima.SetBool("Jump", false);
        }
        if (isStun || isDead)
            return;

        if (buffs.Any(x => x is HitBuff))
            return;
        if (isJumpFrequency)
            lastJumpLandTime += Time.deltaTime;

        if (jump && (isGrounded || isTouchingSlope || isInWater) && !hasJump && lastJumpLandTime >= jumpFrequency)
        {
            ridbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            hasJump = true;
            isJumpFrequency = false;
            anima.SetBool("Jump", true);
            if (isAtMaxSpeed)
            {
                currentGas = 0;
                var addVelocityValue = (jumpBonusToVelocity - 1) * ridbody.velocity.magnitude;
                var addVelocityDir = ridbody.velocity.normalized;
                var addForceValue = ridbody.mass * addVelocityValue / Time.fixedDeltaTime;
                ridbody.AddForce(addVelocityDir*addForceValue,ForceMode.Force);
            }
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
        if (isStun || isDead)
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
        if (isStun || isDead)
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

    private void CheckHP()
    {
        if (currentHPValue < dangerHpTip)
        {
            var rendererBlock = new MaterialPropertyBlock();
            skinnedMeshRenderer.GetPropertyBlock(rendererBlock, 1);
            rendererBlock.SetFloat("_TintAmount", blinkCurve.Evaluate(Time.time));
            skinnedMeshRenderer.SetPropertyBlock(rendererBlock, 1);
        }
        else
        {
            var rendererBlock = new MaterialPropertyBlock();
            skinnedMeshRenderer.GetPropertyBlock(rendererBlock, 1);
            if (rendererBlock.GetFloat("_TintAmount") != 0)
            {
                rendererBlock.SetFloat("_TintAmount", 0f);
                skinnedMeshRenderer.SetPropertyBlock(rendererBlock, 1);
            }

        }
    }

    private void MoveRoll()
    {
        if (isDead)
            return;
        if (isStun)
        {
            if(ridbody.velocity.magnitude > stunStopRollMinVelocity)
            {
                rollRotationAxis = - Vector3.Cross(groundNormal, ridbody.velocity);
                rollRotationAmount = ridbody.velocity.magnitude;
                IKObject.transform.Rotate(rollRotationAxis, - rollRotationAmount, Space.World);
                isRollContinu = true;
            }
            else
            {
                var upAngle = Vector3.Angle(IKObject.transform.up, groundNormal);
                var forwardAngle = Vector3.Angle(IKObject.transform.forward, groundNormal);
                isRollContinu = upAngle < stunStopRollMinAngle || (180f - upAngle) < stunStopRollMinAngle || (stunStopRollMaxAngle < upAngle && upAngle < (180 - stunStopRollMaxAngle))&&(stunStopRollMaxAngle < forwardAngle && forwardAngle < (180 - stunStopRollMaxAngle));
                //����ֹͣ��תʱ��ƽ�ⲹ��
                if(isRollContinu)
                {
                    IKObject.transform.Rotate(rollRotationAxis, -rollRotationAmount, Space.World);

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
        if (isDead)
            return;
        if (isStun)
        {
            lastStunTime += Time.fixedDeltaTime;
            if (lastStunTime >= stunRecoverTime)
            {
                maxStunValue = Math.Max(stunMinValue, maxStunValue - stunDecreaseRate);
                currentStunValue = maxStunValue;
                currentGas = 0f;

                IKObject.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f).onComplete += () =>
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
        if (isDead)
        {
            grounderQuadruped.weight = 0;
            return;
        }
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
        if (isDead)
            return;
        if (floatObj.InWater && !isInWater)
        {
            //��ˮ�ж�
            lastInWaterTime = 0;
        }

        isInWater = floatObj.InWater;

        if (isInWater)
        {
            lastInWaterTime += Time.fixedDeltaTime;
        }

    }

    private void CheckIsGrounded()
    {
        isGrounded = Physics.CheckSphere(bodyCollider.transform.position + (bodyCollider as SphereCollider).center - new Vector3(0, (bodyCollider as SphereCollider).radius, 0), 0.02f, groundMask);
        anima.SetBool("OnGround", isGrounded || isTouchingSlope);
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
        if (!gameController.debug)
        {
            stunSlider.gameObject.SetActive(false);
            hpSlider.gameObject.SetActive(false);
        }
        else
        {
            stunSlider.gameObject.SetActive(true);
            hpSlider.gameObject.SetActive(true);
        }
        gpSlider.value = (float)(currentGas / maxActorGas);
        stunSlider.value = (float)(currentStunValue / maxStunValue);
        hpSlider.value = (float)(currentHPValue / maxHPValue);
        canvas.transform.forward = Camera.main.transform.forward;
        gpSlider.transform.position = this.transform.position;
        stunSlider.transform.position = this.transform.position;
        hpSlider.transform.position = this.transform.position;
        gpSlider.transform.localPosition = gpSlider.transform.localPosition + new Vector3(0, 1.25f + (this.transform.localScale.x - 1) * 1.2f, 0);
        stunSlider.transform.localPosition = stunSlider.transform.localPosition + new Vector3(0, 1.5f + (this.transform.localScale.x - 1) * 1.2f, 0);
        hpSlider.transform.localPosition = hpSlider.transform.localPosition + new Vector3(0, 1.75f + (this.transform.localScale.x - 1) * 1.2f, 0);
        playerIndicator.transform.position = this.transform.position;
        playerIndicator.transform.localPosition = playerIndicator.transform.localPosition + new Vector3(0, 1.75f + (this.transform.localScale.x - 1) * 1.5f, 0);
    }
    private void SetRingColor()
    {
        //var rendererBlock = new MaterialPropertyBlock();
        //ringRenderer.GetPropertyBlock(rendererBlock, 0);
        //rendererBlock.SetColor("_Color", InputReadManager.Instance.playerColors[playerIndex]);
        //ringRenderer.SetPropertyBlock(rendererBlock, 0);
        playerIndexText.text = $"P{playerIndex+1}";
        playerIndicator.GetComponent<Image>().sprite = InputReadManager.Instance.playerIndicatorSprites[playerIndex];
    }

    // temp, �����û����������
    private void SetRingMaxColor()
    {
        //var rendererBlock = new MaterialPropertyBlock();
        //ringRenderer.GetPropertyBlock(rendererBlock, 0);
        //rendererBlock.SetColor("_Color", Color.red);
        //ringRenderer.SetPropertyBlock(rendererBlock, 0);
    }


    #endregion

    #region Render

    #endregion


    #region OnCollison
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<TimeLapseBombSkill>())
        {
            //��ը��
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

            var m1 = (Mathf.Cos(degree1) * vel1).magnitude; //����ԶԷ�����
            var m2 = (Mathf.Cos(degree2) * vel2).magnitude; //�Է�����ҵ���

            var m = m1 + m2;

            var lglooNerfRate = 1f;
            if (hasLglooStun())
            {
                lglooNerfRate = 0.5f;
            }

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
                //Debug.LogError($" ===> {this.gameObject.name} === > ��һ֡ ʵ�ʾ��룺{(this.transform.position - hitPosition).magnitude} ��ǰ�ٶ�:{this.ridbody.velocity.magnitude} Ŀ����룺{hitDistance} ��ʱ: {((DateTime.Now - hitTime).TotalSeconds)}");
                isFirstFrame = false;
                hasPrint = true;
            }
            isFirstFrame = true;
            if ((DateTime.Now - hitTime).TotalSeconds > 0.2f && this.ridbody.velocity.magnitude < 0.1f)
            {
                //Debug.LogError($" ===> {this.gameObject.name} === > ʵ�ʾ��룺{(this.transform.position - hitPosition).magnitude} ��ǰ�ٶ�:{this.ridbody.velocity.magnitude} Ŀ����룺{hitDistance} ��ʱ: {((DateTime.Now - hitTime).TotalSeconds)}");
                isRecordingHit = false;
            }
        }
    }

    /// <summary>
    /// ������˾���
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
        //Debug.LogError($"====> {this.gameObject.name} === > ����:{distance} ��ǰ�ٶ�:{this.ridbody.velocity.magnitude} ����VO��{desiredV0}");
        return new KnockBackForceStruct {
            force = acceleration * ridbody.mass,
            hitTime = Mathf.Abs(desiredV / frictionForceAcceleration)
        };
    }

    /// <summary>
    /// �ڿ��еĻ��˾���
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
                //����״̬����
                var riseTime = currentVelocity.y / currentGravity.magnitude;
                var dropHeight = hit.distance + currentGravity.magnitude * Mathf.Pow(riseTime, 2.0f);
                var dropTime = Mathf.Sqrt(2 * dropHeight / currentGravity.magnitude);


                var initialVelocity =Mathf.Sqrt(frictionForceAcceleration*(frictionForceAcceleration * (Mathf.Pow (dropTime + riseTime,2)+ 2 * distance))) -(dropTime + riseTime)* frictionForceAcceleration;
                var initialVelocityDelta = initialVelocity + magnitudeDeltaV;

                var acceleration = initialVelocityDelta / Time.fixedDeltaTime;
                hitTime = riseTime + dropTime + Mathf.Abs(initialVelocity / frictionForceAcceleration);
                forceMagnitude = acceleration * ridbody.mass;
                //Debug.LogError($"====>���� {this.gameObject.name} === > ����:{distance} ��ǰ�ٶ�:{this.ridbody.velocity.magnitude} ����VO��{initialVelocity}");
            }
            else
            {
                //�½�״̬����
                var riseTime = Mathf.Abs(currentVelocity.y) / currentGravity.magnitude;
                var dropHeight = hit.distance + currentGravity.magnitude * Mathf.Pow(riseTime, 2.0f);
                var dropMaxTime = Mathf.Sqrt(2 * dropHeight / currentGravity.magnitude);
                var dropTime = dropMaxTime - riseTime;
                var initialVelocity = Mathf.Sqrt(frictionForceAcceleration * (frictionForceAcceleration * (Mathf.Pow(dropTime, 2) + 2 * distance))) - (dropTime) * frictionForceAcceleration;
                var initialVelocityDelta = initialVelocity + magnitudeDeltaV;


                hitTime = dropTime + Mathf.Abs(initialVelocity / frictionForceAcceleration);
                forceMagnitude = initialVelocityDelta / Time.fixedDeltaTime * ridbody.mass;
                //Debug.LogError($"====>�½� {this.gameObject.name} === > ����:{distance} ��ǰ�ٶ�:{this.ridbody.velocity.magnitude} ����VO��{initialVelocity}");
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
        //ײ���������崦��
        if (collision.gameObject.GetComponent<InteractiveObject>())
        {

            //�����ٶ�
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

            var eventObjectPrefab = Resources.Load<GameObject>("MediumHit");
            var eventObjectGameObject = Instantiate(eventObjectPrefab, collision.contacts[0].point, Quaternion.Euler(new Vector3(0, 0, 0)));
            var hitDir = Vector3.ProjectOnPlane((ridbody.position - collision.contacts[0].point), groundNormal).normalized;
            var targetDistance = Math.Min(hitMaxDistance, collision.gameObject.GetComponent<InteractiveObject>().knockbackDistance * myHitKnockbackCoef + hitKnockbackSelfCurve.Evaluate(momentumSelf * myBuff + 2)) * InteractiveDistanceCoef;
            var forceData = KnockBackForce(targetDistance, hitDir);
            var targetStun = targetDistance * distanceToStunCoef * InteractiveStunDistanceCoef;
            TakeStun((int)(targetStun));
            var buff = new HitBuff(this, forceData.hitTime);
            ridbody.AddForce((forceData.force) * hitDir, ForceMode.Force);
            this.buffs.Add(buff);
            var hitOnPlane = Vector3.ProjectOnPlane((collision.contacts[0].point - ridbody.position), groundNormal).normalized;
            var forwardOnPlane = Vector3.ProjectOnPlane(ridbody.transform.forward, groundNormal).normalized;
            var hitAngle = Vector3.SignedAngle(forwardOnPlane, hitOnPlane, groundNormal);
            anima.SetFloat("hitAngle", hitAngle);
            anima.SetBool("isHit", true);
            
        }
        //ײ����ɫ����
        if (collision.gameObject.GetComponent<CharacterContorl>())
        {
            isCollidingCharacter = true;
            //��Ч
            var eventObjectPrefab = Resources.Load<GameObject>("MediumHit");
            var eventObjectGameObject = Instantiate(eventObjectPrefab, collision.contacts[0].point, Quaternion.Euler(new Vector3(0, 0, 0)));

            var otherCollision = collision.gameObject.GetComponent<CharacterContorl>();
            //�����ٶ�
            Vector3 velocitySelf = new Vector3(velocityBeforeCollision.x, velocityBeforeCollision.y, velocityBeforeCollision.z);
            velocitySelf = Vector3.ProjectOnPlane(velocitySelf, groundNormal).normalized * velocitySelf.magnitude;
            //�Է��ٶ�
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
            //Debug.Log($"{transform.name} velocitySelf:{velocitySelf} velocityOther:{velocityOther} angleSelf:{angleSelf} angleOther:{angleOther} momentumSelf:{momentumSelf}  momentumOther:{momentumOther}");

            var lglooNerfRate = 1f;
            if (hasLglooStun())
            {
                lglooNerfRate = 0.5f;
            }
            //���мӳ�
            var hasBuff = (otherCollision.isAtMaxSpeed && !otherCollision.isGrounded) ? buffAttack : 1;
            var myBuff = isAtMaxSpeed && !isGrounded ? buffAttack : 1;

            var hitDir = Vector3.ProjectOnPlane((ridbody.position - collision.contacts[0].point), groundNormal).normalized;
            var myHitKnockback = hitKnockbackCurve.Evaluate(momentumSelf * myBuff);
            var otherHitKnockback = hitKnockbackCurve.Evaluate(momentumOther * hasBuff);

            KnockBackForceStruct forceData;

            var targetDistance = Math.Min(hitMaxDistance, hitKnockbackCurve.Evaluate(momentumOther * hasBuff) + hitKnockbackSelfCurve.Evaluate(momentumSelf ));

            //ʩ��ˮƽ����
            if (isGrounded || isTouchingSlope)
                forceData = KnockBackForce(targetDistance, hitDir);
            else
                forceData = KnockBackOnAirForce(targetDistance, hitDir);
           
            var hitOnPlane = Vector3.ProjectOnPlane((collision.contacts[0].point - ridbody.position), groundNormal).normalized;
            var forwardOnPlane = Vector3.ProjectOnPlane(ridbody.transform.forward, groundNormal).normalized;
            var hitAngle = Vector3.SignedAngle(forwardOnPlane, hitOnPlane, groundNormal);
            anima.SetFloat("hitAngle", hitAngle);
            if (myHitKnockback > 1 || otherHitKnockback > 1)
            {
                anima.SetBool("isHit", true);
            }
            //Debug.LogError($"==> {this.gameObject.name} ===> ������� {targetDistance} otherAtMaxSpeed? {otherCollision.isAtMaxSpeed} ");
            //���ѣ�κ�Ѫ��
            TakeStun((int)(hitKnockbackCurve.Evaluate(momentumOther * hasBuff) * distanceToStunCoef));
            TakeDamage((int)(hitKnockbackCurve.Evaluate(momentumOther * hasBuff) * distanceToHPCoef), hitDir);
            //TakeDamage(100, hitDir);//����
            var buff = new HitBuff(this,forceData.hitTime);
            ridbody.AddForce((forceData.force) * hitDir, ForceMode.Force);
            this.buffs.Add(buff);

            //ʩ��ת���� ��ֵ˳ʱ��ת������ֵ��ʱ��ת��
            var torgueAngle = Vector3.SignedAngle(velocityOther, contactToOther, groundNormal);
            if (torgueAngle >= 0)
            {
                ridbody.AddRelativeTorque(Vector3.up * velocityOther.magnitude * Mathf.Cos(torgueAngle * Mathf.Deg2Rad)*50  , ForceMode.Force);
            }
            else
            {
                ridbody.AddRelativeTorque(Vector3.down * velocityOther.magnitude * Mathf.Cos(torgueAngle * Mathf.Deg2Rad)*50 , ForceMode.Force);
            }

            //Debug.LogError($"���� {otherCollision.gameObject.name} force {force} hit Dir {hitDir}");


            //����Է���ʩ����������ʩ��
            if (otherCollision.skill && otherCollision.isUseSkill)
            {
                otherCollision.skill.ExitUseMode();
            }

            //��ת
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
