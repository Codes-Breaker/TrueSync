using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CharacterManager : MonoBehaviour
{
    [Header("������")]
    public InputReaderBase inputReader;
    [Space(10)]
    [Header("����")]
    public float movementSpeed;
    public float jumpForce;
    public float releaseSpeed;
    public float maxScale;
    [Tooltip("Lerp�������")]
    public float chargeTime; 
    [Tooltip("Lerp�������")]
    public float releaseTime;

    //���������
    private Vector2 axisInput;
    private float cameraInputX;
    private bool jump;
    private bool charge;

    //ҡ������ֵ��Сֵ
    private float movementThrashold = 0.01f;
    [Space(10)]
    [Header("�����Ҫ�������")]
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


    // ���ز���
    [HideInInspector]
    public bool isWalk;
    [HideInInspector]
    public bool isGrounded;
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

    private float deltaScale;

    private Rigidbody ridbody;


    private void Awake()
    {
        ridbody = GetComponent<Rigidbody>();
        currentHPValue = maxHPValue;
        deltaScale = (maxScale - 1) / maxActorGas;
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

        MoveCharge();
        MoveRelease();
        SetState();
        MoveWalk();
        MoveJump();
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
        if (jump && isGrounded)
        {
            ridbody.AddForce(Vector3.up * jumpForce,ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void MoveCharge()
    {
        if(charge)
        {
            //if (ridbody.transform.localScale.x < maxScale && !releasing)
            //{
            //    ridbody.transform.localScale = Vector3.Lerp(ridbody.transform.localScale, new Vector3(maxScale, maxScale, maxScale), 0.1f);
            //    neckPoint.targetRotation = Quaternion.Euler(0, 0, -45);
            //    releasing = false;
            //}

            if(currentGas < maxActorGas && !releasing)
            {
                currentGas = currentGas + (maxActorGas - currentGas) / chargeTime * Time.fixedDeltaTime;
                neckPoint.targetRotation = Quaternion.Euler(0, 0, -45);
                releasing = false;
            }
        }
    }

    private void MoveRelease()
    {
        if(!charge)
        {
            //if (ridbody.transform.localScale.x > 1.0f)
            //{
            //    if (ridbody.transform.localScale.x - 1.0f < 0.02f)
            //    {
            //        ridbody.transform.localScale = new Vector3(1, 1, 1);
            //        releasing = false;
            //        releaseEffect.gameObject.SetActive(false);
            //    }
            //    else if (ridbody.transform.localScale.x > 1f)
            //    {
            //        if (!releasing)
            //        {
            //            ridbody.AddForce(ridbody.transform.right * releaseSpeed * 50.0f);
            //        }
            //        else
            //        {
            //            ridbody.AddForce(ridbody.transform.right * releaseSpeed * 2.0f);
            //        }
            //        ridbody.transform.localScale = Vector3.Lerp(ridbody.transform.localScale, new Vector3(1.0f, 1.0f, 1.0f), 0.01f);
            //        neckPoint.targetRotation = Quaternion.Euler(0, 0, 0);
            //        releaseEffect.gameObject.SetActive(true);
            //        releasing = true;
            //    }
            //    else
            //    {
            //        ridbody.transform.localScale = Vector3.Lerp(ridbody.transform.localScale, new Vector3(1.0f, 1.0f, 1.0f), 0.01f);
            //        neckPoint.targetRotation = Quaternion.Euler(0, 0, 0);
            //        ridbody.AddForce(ridbody.transform.right * releaseSpeed * 2.0f);
            //    }
            //}
            if(currentGas > 0)
            {
                var releaseDir = new Vector3(ridbody.transform.right.x, 0, ridbody.transform.right.z);
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