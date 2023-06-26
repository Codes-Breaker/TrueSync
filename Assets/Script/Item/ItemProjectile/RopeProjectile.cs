using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class RopeProjectile : ItemProjectileBase
{
    [Header("扯断时间")]
    public float tearTime;
    [Header("发射扯断时间")]
    public float shootTearTime;
    private string ropePrefabPath = "Prefabs/Item/ItemProjectile/RopeProjectile/Rope";
    public GameObject ropeObject;
    private ObiParticleAttachment startPoint;
    private ObiParticleAttachment endPoint;
    public ObiRigidbody obiRidbody;
    private GameObject removeRope;
    private ObiRope obiRope;
    public ObiSolver solver;
    private ObiRopeBlueprint blueprint;
    public int particlePoolSize = 100;

    private float currentTime;
    private bool isTear;
    private bool isStartStick;
    private bool canStickyCharacter;

    public override void Init(CharacterContorl character,Vector3 project)
    {
        base.Init(character, project);
    }

    public override void Launch()
    {
        base.Launch();
        ropeObject = Instantiate(Resources.Load<GameObject>(ropePrefabPath));
        obiRope = ropeObject.GetComponent<ObiRope>();
        obiRope.stretchingScale = 2f;
        ropeObject.transform.SetParent(GameObject.FindObjectOfType<ObiSolver>().transform);
        solver = GameObject.FindObjectOfType<ObiSolver>();
        var detalPosition = (character.itemPlaceHand.position - transform.position) / (obiRope.activeParticleCount - 1);
        for (int i = 0; i < obiRope.activeParticleCount; i++)
        {
            obiRope.solver.positions[obiRope.solverIndices[i]] = obiRope.solver.transform.InverseTransformPoint(character.itemPlaceHand.position - detalPosition * i);
        }

        var attachments = ropeObject.GetComponents<ObiParticleAttachment>();
        startPoint = attachments[0];
        endPoint = attachments[1];
        endPoint.target = character.itemPlaceHand;
        startPoint.target = transform;
        canStickyCharacter = true;
        isTear = false;
        //isStartStick = false;
        currentTime = 0;

    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        currentTime += Time.deltaTime;
        if (isTear)
        {
            obiRope.enabled = true;
            RemoveRidgidBody();
        }

        
        if (isStartStick)
        {
            //obiRope.enabled = true;
            isStartStick = false;
            Destroy(removeRope);
        }
        if (currentTime >= shootTearTime && !isTear)
        {
            obiRope.enabled = false;
            startPoint.target = null;
            endPoint.target = null;
            isTear = true;
        }
    }

    private void RemoveRidgidBody()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<SphereCollider>().enabled = false;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        var otherCharacter = collision.collider.GetComponent<CharacterContorl>();
        if (otherCharacter && canStickyCharacter && otherCharacter != character)
        {
            if (otherCharacter.invulernable)
            {
                OnTouchGround();
                return;
            }
            startPoint.target = otherCharacter.transform;
            canStickyCharacter = false;

            var ropeStun = new QTERopeStun(otherCharacter, character, tearTime, 0.05f);
            otherCharacter.OnGainBuff(ropeStun);

            //ropeObject.SetActive(false);
            Destroy(ropeObject);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //var otherCharacter = collision.GetComponent<Collider>().GetComponent<CharacterContorl>();
        //if (otherCharacter && canStickyCharacter && otherCharacter != character)
        //{
        //    startPoint.target = otherCharacter.transform;
        //    canStickyCharacter = false;

        //    removeRope = ropeObject;
        //    //ropeObject.SetActive(false);
        //    ropeObject = Instantiate(Resources.Load<GameObject>(ropePrefabPath));
        //    obiRope = ropeObject.GetComponent<ObiRope>();
        //    ropeObject.transform.SetParent(GameObject.FindObjectOfType<ObiSolver>().transform);
        //    var detalPosition = (character.transform.position - otherCharacter.transform.position) / (obiRope.activeParticleCount - 1);
        //    for (int i = 0; i < obiRope.activeParticleCount; i++)
        //    {
        //        obiRope.solver.positions[obiRope.solverIndices[i]] = obiRope.solver.transform.InverseTransformPoint(character.transform.position - detalPosition * i);
        //        //obiRope.solver.positions[obiRope.solverIndices[i]] =character.transform.position;

        //    }
        //    var attachments = ropeObject.GetComponents<ObiParticleAttachment>();
        //    startPoint = attachments[0];
        //    endPoint = attachments[1];
        //    endPoint.target = character.transform;
        //    startPoint.target = otherCharacter.transform;
        //    // obiRope.enabled =false;
        //    isStartStick = true;
        //}
    }

    protected override void OnTouchGround()
    {
        base.OnTouchGround();
        //obiRidbody.kinematicForParticles = false;
        if (canStickyCharacter)
        {
            endPoint.target = null;
            //startPoint.target = null;
            canStickyCharacter = false;
            obiRope.enabled = false;
            isTear = true;
        }
    }
    public override void OnEnd()
    {
        base.OnEnd();
    }

}
