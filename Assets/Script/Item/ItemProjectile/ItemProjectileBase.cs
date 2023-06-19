using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider),typeof(Rigidbody))]
public class ItemProjectileBase : MonoBehaviour
{
    public LayerMask groundLayer;
    protected CharacterContorl character;
    //飞行目标方向
    protected Vector3 project;
    //水平初速度
    public float initialHorizontalSpeed;
    //向上初速度
    public float initialVerticalSpeed;
    //弹道下坠速度
    public float projectileGravity = 1f;

    protected Rigidbody rb;
    protected Collider bodyCollider;
    private bool hasThrow = false;
    public virtual void Init(CharacterContorl character)
    {
        this.character = character;
        bodyCollider = gameObject.GetComponent<Collider>();
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
       // bodyCollider.isTrigger = true;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        
    }

    protected virtual void FixedUpdate()
    {
        AddGravity();
        CheckGround();
    }

    public virtual void Launch()
    {

        this.project = this.character.transform.forward;
        hasThrow = true;
        this.transform.parent = null;
        rb.isKinematic = false;
        rb.velocity = (character.ridbody.velocity.magnitude + initialHorizontalSpeed) * project + initialVerticalSpeed * Vector3.up;
    }

    private void OnDestroy()
    {
        character.animationEventReceiver.UnRegisterEvent(AnimationEventReceiver.EventEnum.ThrowBoom, Throw);
    }


    public virtual void Throw()
    {
        this.transform.parent = character.itemPlaceHand;
        this.transform.localPosition = Vector3.zero;
        rb.isKinematic = true;
        Physics.IgnoreCollision(this.bodyCollider, character.bodyCollider, true);


    }

    private void AddGravity()
    {
        if (hasThrow)
            rb.AddForce(Vector3.down * projectileGravity * rb.mass);
    }

    private void CheckGround()
    {
        if(Physics.CheckSphere(bodyCollider.transform.position + (bodyCollider as SphereCollider).center * bodyCollider.transform.localScale.y - new Vector3(0, (bodyCollider as SphereCollider).radius * bodyCollider.transform.localScale.y, 0), 0.1f, groundLayer) && hasThrow)
            OnTouchGround();

    }

    protected virtual void OnTouchGround()
    {
    }

    public virtual void OnEnd()
    {
        Destroy(gameObject);
    }
}
