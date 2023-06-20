using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class RocketProjectile : ItemProjectileBase
{
    private Vector3 initialVelocity = Vector3.zero;
    public Quaternion itemRot = Quaternion.identity;
    public Vector3 rocketForward = Vector3.zero;
    private float runMaxVelocity = 12f;
    public float explosionRangeRadius = 5f;
    public float explosionForceArgument = 2000f;
    private float autoExplosionTime = 20f;
    private float elapsedTime = 0f;
    //爆炸特效预制体路径
    private string timeLapseBombExplosionEffectPath = "Prefabs/Effect/StickyBomb_Explosion";
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //Quaternion rotation = Quaternion.Euler(0f, -90f, 0f);
        //initialVelocity = (rotation * this.transform.forward) * runMaxVelocity;
        //initialVelocity = new Vector3 (transform.forward.z,-transform.forward.x,-transform.forward.y) * runMaxVelocity;
        rb.velocity = initialVelocity;
        elapsedTime += Time.fixedDeltaTime;
        if (elapsedTime > autoExplosionTime)
        {
            Explosion();
        }
    }

    public override void Init(CharacterContorl character)
    {
        base.Init(character);
        rb.useGravity = false;
    }

    public override void Launch()
    {

    }

    /// <summary>
    /// 结算击退距离
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    private float KnockBackForce(float distance, Vector3 hitDir)
    {
        var gravityDivide = Vector3.ProjectOnPlane(Physics.gravity, Vector3.up) * rb.mass;
        var gravityFrictionDivide = Physics.gravity - gravityDivide;
        var frictionForceAcceleration = bodyCollider.material.dynamicFriction * gravityFrictionDivide.magnitude;


        //var deltaV = Vector3.ProjectOnPlane((this.ridbody.velocity - velocityBeforeCollision), groundNormal);
        var deltaV = Vector3.ProjectOnPlane(rb.velocity, Vector3.up);
        var magnitudeDeltaV = Vector3.Dot(deltaV, hitDir.normalized);

        var desiredV = (float)Math.Sqrt((2 * frictionForceAcceleration * distance));

        var desiredV0 = desiredV - magnitudeDeltaV;
        var acceleration = desiredV0 / Time.fixedDeltaTime;
        //Debug.LogError($"====> {this.gameObject.name} === > 距离:{distance} 当前速度:{this.ridbody.velocity.magnitude} 理论VO：{desiredV0}");
        return acceleration * rb.mass;
    }



    protected override void OnCollisionEnter(Collision collision)
    {
        ////施加转角力 正值顺时针转动，负值逆时针转动
        //var torgueAngle = Vector3.SignedAngle(initialVelocity, collision.contacts[0].point, Vector3.up);
        //if (torgueAngle >= 0)
        //{
        //    rb.AddRelativeTorque(Vector3.up * initialVelocity.magnitude * Mathf.Cos(torgueAngle * Mathf.Deg2Rad) * 50, ForceMode.Force);
        //}
        //else
        //{
        //    rb.AddRelativeTorque(Vector3.down * initialVelocity.magnitude * Mathf.Cos(torgueAngle * Mathf.Deg2Rad) * 50, ForceMode.Force);
        //}

        //var hitDir = Vector3.ProjectOnPlane((rb.position - collision.contacts[0].point), Vector3.up).normalized;
        //var force = KnockBackForce(2, hitDir);
        //rb.AddForce((force) * hitDir, ForceMode.Force);

        if (collision.transform.tag == "StaticObject")
        {
            Explosion();
        }

    }

    private void Explosion()
    {
        var colliders = GameObject.FindObjectsOfType<Rigidbody>();

        if (colliders.Length != 0)
        {
            foreach (var item in colliders)
            {
                if ((item.transform.position - this.transform.position).magnitude < explosionRangeRadius)
                {
                    item.GetComponent<Rigidbody>().AddExplosionForce(explosionForceArgument, this.transform.position, explosionRangeRadius);
                }
            }

        }
        var effectPrefab = Resources.Load(timeLapseBombExplosionEffectPath);
        var effectGameObject = (GameObject)GameObject.Instantiate(effectPrefab, this.transform.position, Quaternion.Euler(Vector3.zero));
        GameObject.Destroy(effectGameObject, 3f);
        base.OnEnd();
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, this.transform.position + initialVelocity * 5);
    }

    public override void Throw()
    {
        this.transform.parent = character.itemPlaceBelly;
        this.transform.localPosition = Vector3.zero;
        this.transform.rotation = itemRot;
        this.transform.parent = null;
        this.project = rocketForward;
        Physics.IgnoreCollision(this.bodyCollider, character.bodyCollider, true);
        rb.velocity = runMaxVelocity * project;
        initialVelocity = rb.velocity;
    }
}

