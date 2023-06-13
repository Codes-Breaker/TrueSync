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
    public float initialHorizontalSpeed ;
    //弹道下坠速度
    public float projectileGravity = 1f;

    protected Rigidbody rb;
    protected  Collider bodyCollider;

    public virtual void Init(CharacterContorl character,Vector3 project)
    {
        this.character = character;
        this.project = project;
        bodyCollider = gameObject.GetComponent<Collider>();
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
       // bodyCollider.isTrigger = true;
        Launch();
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    protected virtual void FixedUpdate()
    {
        AddGravity();
        CheckGround();
    }

    private void Launch()
    {
        rb.velocity = initialHorizontalSpeed * project;
    }

    private void AddGravity()
    {
        rb.AddForce(Vector3.down * projectileGravity * rb.mass);
    }

    private void CheckGround()
    {
        if(Physics.CheckSphere(bodyCollider.transform.position + (bodyCollider as SphereCollider).center - new Vector3(0, (bodyCollider as SphereCollider).radius * bodyCollider.transform.localScale.y, 0), 0.02f, groundLayer))
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
