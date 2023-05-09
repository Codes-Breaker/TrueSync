using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using Cinemachine;

public class CharacterContorl : MonoBehaviour
{
    [Header("������")]
    public InputReaderBase inputReader;
    [Space(10)]
    [Header("����")]
    [Header("��·��ϵ��")]
    public float movementForce;
    [Header("��������ٶ�")]
    public float maxWalkVelocity;
    [Header("��Ծ��С��ϵ��")]
    public float jumpMinForce;
    [Header("��Ծ�����")]
    public float jumpMaxForce;
    [Header("��Ծ����ʱ��")]
    public float jumpChargeTime;
    [Header("������ʼ��ϵ��")]
    public float releaseSpeedAtFirstArgument;
    [Header("��������������ϵ��")]
    public float releaseSpeedLinearArgument;
    [Header("������С���ٶ�ϵ��")]
    public float minLinearReleaseSpeedArgument;
    [Header("������ϵ��ϵ��")]
    public float decelerationForceArgument;
    [Header("����Ť����ϵ��ϵ��")]
    public float decelerationTorqueArgument;
    [Header("�����Layers")]
    [SerializeField] public LayerMask groundMask;
    [Tooltip("Lerp�������")]
    public float chargeTime;
    [Tooltip("Lerp�������")]
    public float releaseTime;
    [Header("б�¾���ײ��ļ�����")]
    public float slopeCheckerThrashold = 0.51f;
    [Header("���б�½Ƕ�")]
    public float maxClimbableSlopeAngle = 40f;
    public float cureTime;

    private bool hasJump = false;
    private bool hasBrake = false;


    //ҡ������ֵ��Сֵ
    private float movementThrashold = 0.35f;
    //public float originalRadius;
    //public float targetRadius;
    //public Vector3 targetCenter;
    //public Vector3 originalCenter;
    public float sensitivity = 0.5f;
    private Vector3 initialRotation;
    private Quaternion initialRot;
    // ���ز���
    private bool isWalk;
    private bool isGrounded;
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
    public float HPtimer;
    [HideInInspector]
    public SkillItemControllerBase skill;
    [HideInInspector]
    public bool isUseSkill;
    //������
    public float forceArgument;
    //������ϵ��
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

    public float maxReleaseVelocity;

    public float maxDrowning = 1000;
    private float maxDrownValue = 1000;
    private float currentJumpTime = 0f;
    public float currentDrown = 0;
    private float totalDrown = 0;
    private const int minDrown = 50;

    [Range(0, 1)]
    public float continueReceivedForceRate = 0.2f; 

    public float invulernableTime = 0;
    public float maxInvulnerableTime = 5;
    //�Ƿ����޵�
    public bool invulernable = false;
    //��ײ�ܻ��ۻ�ֵ
    public float vulnerbility = 0f;
    //�������ֵ
    public float maxVulnerbility = 25.0f;
    //���ڼ��������б��
    private Vector3 groundNormal;
    private bool isTouchingSlope = false;

    [Range(0, 1)]
    //����ϵ��
    public float vulerbilityReactRate = 1;
    ////�Ƿ����ڷ���
    //public bool returning = false;
    ////�Ƿ�������Ծ
    //public bool jumpingBack = false;
    ////��ӾĿ�ĵ�
    //public Vector3 swimTarget = Vector3.zero;
    ////��ԾĿ�ĵ�
    //public Vector3 jumpTarget = Vector3.zero;
    ////��Ӿ�ٶ�
    //public float swimSpeed = 1;

    public bool isDead = false;

    private int defaultLayer = 0;

    //��¼ֵ
    public float receivedForceSum = 0;
    //ѣ�β�
    public float stunTime = 0;
    //ѣ��ʱ��
    public float maxStunTime = 0;
    //�Ƿ�ѣ��
    private bool isStun = false;
    [Space(10)]
    [Header("�����Ҫ�������")]
    public TMP_Text vulnerbilityText;
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
    //��Ч��Դ
    public GameObject stunEffect;
    //ѣ����ֵ
    public float stunThreshold;

    public List<Buff> buffs = new List<Buff>();

    private GameController gameController;

    public int playerIndex = -1;

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

    //private void OnDrawGizmos()
    //{
    //    if (this.ridbody != null)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawLine(this.ridbody.position, swimTarget);
    //        Gizmos.color = Color.blue;
    //        Gizmos.DrawLine(this.ridbody.position, jumpTarget);
    //    }
    //}

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


    private void FixedUpdate()
    {
        velocityBeforeCollision = GetComponent<Rigidbody>().velocity;
        positionBeforeCollision = GetComponent<Rigidbody>().position;
        CheckInVulernable();
        CheckIsGrounded();
        UpdateBuff();
        CheckSlopeAndDirections();
       // BalanceGravity();

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

   //ƽ��б����Ħ����
    private void BalanceGravity()
    {
        float gravitationalForce = Mathf.Abs(Physics.gravity.y) * ridbody.mass;
        Vector3 verticalGravity = Vector3.Project(-Physics.gravity, groundNormal);
        Vector3 horizontalGravity = -Physics.gravity - verticalGravity;
        //0.5fʱб��Ħ����ϵ��
        Vector3 frictionForce = -horizontalGravity.normalized * gravitationalForce * 0.5f;
        Vector3 totalForce = verticalGravity + horizontalGravity + frictionForce;


        ridbody.AddForce(-totalForce, ForceMode.Force);

    }


    //public void SetUpJump()
    //{
    //    startPos = ridbody.transform.position;
    //    HDist = (jumpTarget - new Vector3(ridbody.transform.position.x, jumpTarget.y, ridbody.transform.position.z)).magnitude;
    //    HDist *= 1.2f;
    //    curTime = 0;
    //    currentGas = 0;
    //    vulnerbility = 0;
    //    releasing = false;
    //    SetKinematics(true);

    //    maxDrowning -= Mathf.Max(totalDrown, minDrown);
    //    maxDrowning = Mathf.Max(0, maxDrowning);
    //    currentDrown = maxDrowning;
    //    totalDrown = 0;

    //    if (maxDrowning <= 0)
    //    {
    //        Dead();
    //    }
    //}

    //private float HDist = 0;
    //private float curTime = 0;
    //private float maxTime = 1f;
    //private Vector3 startPos;
    //private float maxHeight = 5;
    //private void JumpToPlace()
    //{
    //    if (!jumpingBack)
    //        return;
    //    curTime += Time.fixedDeltaTime;
    //    var hDelta = jumpTarget - startPos;
    //    hDelta.y = 0;
    //    var hPos = Mathf.Lerp(0, HDist, curTime/ maxTime) * hDelta.normalized;
    //    var v = Mathf.Lerp(0, maxHeight, Mathf.Sin(Mathf.Lerp(0, (3f / 4f) * Mathf.PI, curTime / maxTime)));
    //    if (curTime <= maxTime/2f)
    //        this.transform.LookAt(jumpTarget);
    //    Vector3 currentPos = startPos + new Vector3(hPos.x, v, hPos.z);
    //    ridbody.transform.position = currentPos;
    //    if (curTime >= maxTime)
    //    {
    //        jumpingBack = false;
    //        SetKinematics(false);
    //        SetCollider(false);
    //        invulernable = true;
    //        invulernableTime = maxInvulnerableTime;
    //        SetFlashMeshRendererBlock(true);
    //    }
    //}

    // �޵���Чʾ��
    //private void SetFlashMeshRendererBlock(bool value)
    //{
    //    meshController.SetFlashMeshRendererBlock(value);
    //}

    private void SetRingColor()
    {
        var rendererBlock = new MaterialPropertyBlock();
        ringRenderer.GetPropertyBlock(rendererBlock, 0);
        rendererBlock.SetColor("_Color", InputReadManager.Instance.playerColors[playerIndex]);
        ringRenderer.SetPropertyBlock(rendererBlock, 0);
    }

    //private void ReturnToPlace()
    //{
    //    //targetAngle = Mathf.Atan2(swimTarget.x, swimTarget.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
    //    //this.ridbody.transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), 0.1f);
    //    //this.transform.LookAt(swimTarget);

    //    Vector3 relativePos = swimTarget - transform.position;
    //    Quaternion toRotation = Quaternion.LookRotation(relativePos);
    //    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 1 * Time.deltaTime);

    //    var dir = (swimTarget - ridbody.transform.position).normalized;
    //    ridbody.transform.position = ridbody.transform.position + (dir * swimSpeed * Time.fixedDeltaTime);
    //    AccumulateDrown();
    //}

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

    private void SetState()
    {
        var gasScale = (currentGas / (maxActorGas * 1.0f));
        //skinnedMeshRenderer.SetBlendShapeWeight(0, gasScale * 100f);
        //(bodyCollider as SphereCollider).radius = Mathf.Lerp(originalRadius, targetRadius, gasScale);
    }

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
        if (controlDeviceType == ControlDeviceType.Mouse)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            var detalPosition = axisInput - new Vector2(screenPosition.x, screenPosition.y);
            axisInput = detalPosition.normalized;
        }
        else
            axisInput = axisInput.normalized;
        if (axisInput.magnitude > movementThrashold )
        {
            targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            if (ridbody.velocity.magnitude < maxWalkVelocity)
            {
                var moveTarget = ridbody.transform.forward;
                moveTarget = moveTarget.normalized;
                ridbody.AddForce(moveTarget * movementForce, ForceMode.Force);
            }
            isWalk = true;
            anima.SetBool("isWalk",isWalk);
        }
        else
        {
            isWalk = false;
            anima.SetBool("isWalk", isWalk);
        }
        this.ridbody.transform.rotation = Quaternion.Slerp(this.ridbody.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0) + initialRotation), 0.1f);

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
        if (jump && (isGrounded||isTouchingSlope) && !hasJump)
        {
            hasJump = true;

        }
        if(hasJump)
        {
            if ((isGrounded || isTouchingSlope))
                currentJumpTime += Time.fixedDeltaTime;
            else
            {
                currentJumpTime = 0;
                hasJump = false;
            }
        }

        if(!jump && (isGrounded||isTouchingSlope) && hasJump)
        {
            var jumpForce = jumpMinForce + (jumpMaxForce - jumpMinForce) * Mathf.Min(1, currentJumpTime / jumpChargeTime);
            ridbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            hasJump = false;
            currentJumpTime = 0;
        }
        

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
                var releaseDir = ridbody.transform.forward;
                releaseDir = releaseDir.normalized;


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
                    var addSpeed = EaseOutCirc(currentGas / maxActorGas) * releaseSpeedLinearArgument;
                    addSpeed = Mathf.Max(minLinearReleaseSpeedArgument, addSpeed);
                    if (m1 < maxReleaseVelocity)
                    {
                        //Debug.LogError($"{this.gameObject.name} ======> {addSpeed}");
                        ridbody.AddForce(releaseDir * addSpeed, ForceMode.Impulse);
                    }

                }

                // currentGas = currentGas - (currentGas) / releaseTime * Time.fixedDeltaTime;
                currentGas = currentGas - (maxActorGas) / releaseTime * Time.fixedDeltaTime;
                currentGas = Mathf.Max(0, currentGas);


                releasing = true;
                
            }
        }
    }

    #endregion

    #region Check

    private void CheckIsGrounded()
    {
        isGrounded = Physics.CheckSphere(bodyCollider.transform.position+(bodyCollider as SphereCollider).center - new Vector3(0, (bodyCollider as SphereCollider).radius , 0), 0.02f, groundMask);
        //float sphereRadius = (bodyCollider as SphereCollider).radius;
        //float sphereDistance = sphereRadius - 0.02f;

        //Vector3 sphereCenter = bodyCollider.transform.position + (bodyCollider as SphereCollider).center - new Vector3(0, sphereRadius, 0);
        //RaycastHit hit;
        //if (Physics.SphereCast(sphereCenter, sphereRadius, Vector3.down, out hit, sphereDistance + 0.01f, groundMask))
        //{
        //    isGrounded = true;
        //}
        //else
        //{
        //    isGrounded = false;
        //}
    }

    private void CheckSlopeAndDirections()
    {
        RaycastHit slopeHit;
        isTouchingSlope = false;
        if(Physics.SphereCast(bodyCollider.transform.position + (bodyCollider as SphereCollider).center, slopeCheckerThrashold,Vector3.down,out slopeHit, (bodyCollider as SphereCollider).radius + 0.2f, groundMask))
        {
            groundNormal = slopeHit.normal;
            if(Vector3.Angle(Vector3.up, slopeHit.normal) < maxClimbableSlopeAngle)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.FromToRotation(transform.up, slopeHit.normal) * transform.rotation,0.2f);
                isTouchingSlope = true;
            }
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(transform.up,Vector3.zero) * transform.rotation, 0.2f);
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
        if (vulnerbilityText != null)
        {
            vulnerbilityText.text = $"{vulnerbility}%";
            if (vulnerbility > maxVulnerbility)
            {
                vulnerbilityText.color = Color.red;
            }
            else
            {
                vulnerbilityText.color = Color.white;
            }
            gpSlider.value = (float)(currentGas / maxActorGas);
            hpSlider.value = (float)(maxDrowning / maxDrownValue);
            canvas.transform.forward = Camera.main.transform.forward;
            vulnerbilityText.transform.position = bodyCollider.transform.position;
            gpSlider.transform.position = bodyCollider.transform.position;
            hpSlider.transform.position = bodyCollider.transform.position;
            vulnerbilityText.transform.localPosition = vulnerbilityText.transform.localPosition + new Vector3(0.25f, 1.5f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
            gpSlider.transform.localPosition = gpSlider.transform.localPosition + new Vector3(0, 1.3f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
            hpSlider.transform.localPosition = hpSlider.transform.localPosition + new Vector3(0, 1.5f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
            drownImage.transform.position = bodyCollider.transform.position;
            drownImage.transform.localPosition = drownImage.transform.localPosition + new Vector3(-1, 1.5f + (bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
            drownImage.fillAmount = (maxDrowning - currentDrown) / maxDrowning;
        }

    }
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
            var d2 = Vector3.Angle(vel1, contactToOther);

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

            ridbody.AddExplosionForce((otherCollision.forceArgument + m2) * (1 + Mathf.Pow(1.5f, (vulnerbility/10))) * continueReceivedForceRate + 200 * lglooNerfRate, collision.contacts[0].point, 4);
            collision.collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce((forceArgument + m1) * (1 + (otherCollision.vulnerbility / otherCollision.maxVulnerbility)) * otherCollision.continueReceivedForceRate + 50, collision.contacts[0].point, 4);

            receivedForceSum += (forceArgument + vulnerbility) * m * 0.2f;
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
            //��ը��
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
            //��Ч
            var eventObjectPrefab = Resources.Load<GameObject>("MediumHit");
            var eventObjectGameObject = Instantiate(eventObjectPrefab, collision.contacts[0].point, Quaternion.Euler(new Vector3(0, 0, 0)));

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

            vulnerbility += Convert.ToInt32(receivedForceRate * m2 * 2);

            if (Convert.ToInt32(receivedForceRate * m2 * 2) != 0)
            {
                DOTween.Kill(vulnerbilityText.transform);
                vulnerbilityText.transform.localScale = new Vector3(1,1,1);
                vulnerbilityText.transform.DOShakeScale(0.6f, vibrato:7,strength:0.6f ).SetEase(Ease.OutQuad);
            }

            var lglooNerfRate = 1f;
            if (hasLglooStun())
            {
                lglooNerfRate = 0.5f;
            }
            ridbody.AddExplosionForce((otherCollision.forceArgument + m2) * (1 + Mathf.Pow(1.5f, (vulnerbility / 10))) + 200 * lglooNerfRate, collision.contacts[0].point, 4);
            collision.collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce((forceArgument + m1) * (1 + (otherCollision.vulnerbility / otherCollision.maxVulnerbility)) + 50, collision.contacts[0].point, 4);

            //����Է���ʩ����������ʩ��
            if(otherCollision.skill && otherCollision.isUseSkill)
            {
                otherCollision.skill.ExitUseMode();
            }
        }

    }
    #endregion

}
