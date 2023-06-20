using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider),typeof(Rigidbody))]
public class ItemProjectileBase : MonoBehaviour
{
    public LayerMask groundLayer;
    protected CharacterContorl character;
    //����Ŀ�귽��
    protected Vector3 project;
    //ˮƽ���ٶ�
    public float initialHorizontalSpeed;
    //���ϳ��ٶ�
    public float initialVerticalSpeed;
    //������׹�ٶ�
    public float projectileGravity = 1f;

    protected Rigidbody rb;
    protected Collider bodyCollider;
    private bool hasThrow = false;
    public virtual void Init(CharacterContorl character,Vector3 project)
    {
        this.character = character;
        this.project = project;
        bodyCollider = gameObject.GetComponent<Collider>();
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
        Launch();
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
        this.transform.parent = null;
        rb.isKinematic = false;
        //var characterGroundNomal = character.groundNormal;
        //var horizontalProject = Vector3.ProjectOnPlane(project, characterGroundNomal).normalized;
        //var verticalProject = Vector3.Cross(project, Vector3.up).normalized;
        rb.velocity = (character.ridbody.velocity.magnitude + initialHorizontalSpeed) * project + initialVerticalSpeed * Vector3.up;
    }

    private void OnDestroy()
    {
       
    }

    private void AddGravity()
    {
        rb.AddForce(Vector3.down * projectileGravity * rb.mass);
    }

    private void CheckGround()
    {
        //if(Physics.CheckSphere(bodyCollider.transform.position + (bodyCollider as SphereCollider).center * bodyCollider.transform.localScale.y - new Vector3(0, (bodyCollider as SphereCollider).radius * bodyCollider.transform.localScale.y, 0), 0.025f, groundLayer))
        if (Physics.OverlapSphere((bodyCollider.transform.position + (bodyCollider as SphereCollider).center * bodyCollider.transform.localScale.y), (bodyCollider as SphereCollider).radius * bodyCollider.transform.localScale.y,groundLayer).Length != 0)
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
