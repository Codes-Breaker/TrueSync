using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LglooControl : MonoBehaviour,IRandomEventsObject
{
    public bool canEnter;
    private bool isShow;
    public float enterAngle;
    public float enterDistance;
    private float targetAngle;
    public float froceArgument;
    public float showDuration;
    private float currentTime;
    private float stayTime;
    public float endDuration;
    private CharacterContorl user;
    private float userY;
  
    public void Init()
    {
        canEnter = false;
    }
    public void FixedUpdate()
    {
        if(canEnter)
        {
            CheckCanEnter();
        }
    }

    public void CheckCanEnter()
    {
        var players = FindObjectsOfType<CharacterContorl>();
        if (players.Length != 0)
        {
        }
    }

    private void Update()
    {
        if (!isShow)
            return;
        currentTime += Time.deltaTime;
        if (currentTime > stayTime)
        {
            OnExit();
        }
    }

    public void OnExit()
    {
        if (user)
            Fire();
        transform.DOLocalMoveY(0f, endDuration).OnComplete(() => {
            gameObject.SetActive(false);
        });
    }

    public void OnShow(Vector3 position,float stayTime)
    {
        this.stayTime = stayTime;
        canEnter = false;
        transform.position = position;
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        this.gameObject.SetActive(true);
        transform.DOLocalMoveY(1.5f, showDuration).OnComplete(()=>{
            canEnter = true;
            isShow = true;
        });
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(canEnter)
        {
            float angle = Vector3.Angle(transform.forward, collision.transform.position - transform.position);
            if ((collision.transform.position - transform.position).magnitude < enterDistance && angle < enterAngle && collision.collider.GetComponent<CharacterContorl>())
            {
                user = collision.collider.GetComponent<CharacterContorl>();

                if(user.skill && user.isUseSkill)
                {
                    user.skill.ExitUseMode();
                }

                SetContorlLgloo(user);
                
                canEnter = false;

                user.SetKinematics(true);

                userY = user.transform.position.y; 

                user.transform.position = transform.position;

            }

        }
    }

    private void SetContorlLgloo(CharacterContorl user)
    {
        user.moveAciotn = MoveWalk;
        user.chargeAction = MoveCharge;
        user.jumpAction = MoveJump;
        user.releaseAciton = MoveRelease;
        user.interactWeaponAction = null;
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
        if (axisInput.magnitude > 0.01f)
        {
            targetAngle = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        }
        this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, targetAngle, 0)), 0.1f);
    }


    private void MoveJump(bool jump)
    {

    }

    private void MoveCharge(bool charge)
    {
        if(charge)
        {
            Fire();
            OnExit();
        }
    }

    private void Fire()
    {
        transform.GetComponent<Collider>().enabled = false;
        user.transform.position = transform.position ;
        user.transform.position = new Vector3(user.transform.position.x, userY, user.transform.position.z);
        user.SetKinematics(false);
        var ridbody = user.GetComponent<Rigidbody>();
        user.targetAngle = targetAngle;
        ridbody.transform.rotation = Quaternion.Euler(new Vector3(0, targetAngle, 0));
        ridbody.AddForce(transform.forward * froceArgument,ForceMode.Impulse);
        user.SetControlSelf();
        user.buffs.Add(new LglooBuff(user));
        user = null;
    }


    private  void MoveRelease(bool chage)
    {

    }
}
