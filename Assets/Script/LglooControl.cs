using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LglooControl : MonoBehaviour,IRandomEventsObject
{
    public bool canEnter;
    public float enterAngle;
    public float enterDistance;
    private float targetAngle;
    public float froceArgument;
    public float showDuration;
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
            //foreach (var item in players)
            //{
            //    float angle = Vector3.Angle(transform.forward, item.transform.position - transform.position);
            //    if ((item.transform.position - transform.position).magnitude < enterDistance && angle < enterAngle)
            //    {
            //        if(item.GetComponent<Rigidbody>())
            //        item.GetComponent<Rigidbody>().AddForce((transform.position - item.transform.position).normalized * 0.7f, ForceMode.Impulse);
            //    }
            //}
        }
    }

    public void OnExit()
    {
        transform.DOLocalMoveY(0f, endDuration).OnComplete(() => {
            this.gameObject.SetActive(false);
        });
    }

    public void OnShow(Vector3 position)
    {
        canEnter = false;
        transform.position = position;
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        this.gameObject.SetActive(true);
        transform.DOLocalMoveY(1.5f, showDuration).OnComplete(()=>{
            canEnter = true;
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
    }

    private void MoveWalk(Vector2 axisInput)
    {
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
            user.transform.position = transform.position + transform.forward * transform.localScale.x;
            user.transform.position = new Vector3(user.transform.position.x, userY, user.transform.position.z);
            user.SetKinematics(false);
            var ridbody = user.GetComponent<Rigidbody>();
            user.targetAngle = targetAngle;
            ridbody.transform.rotation = Quaternion.Euler(new Vector3(0, targetAngle, 0));
            ridbody.AddForce(transform.forward * froceArgument,ForceMode.Impulse);
            user.SetControlSelf();
            user = null;

            OnExit();
        }
    }


    private  void MoveRelease(bool chage)
    {

    }
}
