using Obi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class QTERopeStun : QTEBuff
{
    private string ropePrefabPath = "Prefabs/Item/ItemProjectile/RopeProjectile/Rope";
    public GameObject ropeObject;
    private ObiParticleAttachment startPoint;
    private ObiParticleAttachment endPoint;
    public ObiRigidbody obiRidbody;
    private GameObject removeRope;
    private ObiRope obiRope;
    private bool isStartStick;
    private bool cutting = false;
    public QTERopeStun(CharacterContorl target) : base(target)
    {

    }

    public QTERopeStun(CharacterContorl target, CharacterContorl source, float buffTime, float qteRate) : base(target, source, buffTime, qteRate)
    {
        QTEAccelerateRate = qteRate;
        requireIKOff = false;
        QTEPriority = 1; //最高优先级
    }

    public override void OnBuffApply()
    {
        character.stun.gameObject.SetActive(true);
        character.anima.SetBool("isStun", true);
        base.OnBuffApply();
        character.ridbody.mass = 1;
        //绳子
        ropeObject = GameObject.Instantiate(Resources.Load<GameObject>(ropePrefabPath));
        obiRope = ropeObject.GetComponent<ObiRope>();
        obiRope.stretchingScale = 1.25f;
        ropeObject.transform.SetParent(GameObject.FindObjectOfType<ObiSolver>().transform);
        var detalPosition = (source.itemPlaceHand.position - character.transform.position) / (obiRope.activeParticleCount - 1);
        for (int i = 0; i < obiRope.activeParticleCount; i++)
        {
            obiRope.solver.positions[obiRope.solverIndices[i]] = obiRope.solver.transform.InverseTransformPoint(source.itemPlaceHand.position - detalPosition * i);
            //obiRope.solver.positions[obiRope.solverIndices[i]] =character.transform.position;

        }
        var attachments = ropeObject.GetComponents<ObiParticleAttachment>();
        startPoint = attachments[0];
        endPoint = attachments[1];
        endPoint.target = source.itemPlaceHand.transform;
        startPoint.target = character.transform;
        // obiRope.enabled =false;
        isStartStick = true;
    }

    public override void OnBuffRemove()
    {
        if (character.countQTEStun() == 1)
        {
            character.stun.gameObject.SetActive(false);
            character.anima.SetBool("isStun", false);
        }

        
        if (character.countRopeStunBuff() == 1)
        {
            character.ridbody.mass = 8;
        }

        base.OnBuffRemove();
    }

    public override void OnBuffUpdate()
    {
        if (cutting)
        {
            obiRope.enabled = true;
            Finish();
            return;
        }
        //特殊流程，因为有个重置
        if (buffTime != -1)
        {
            buffTime = buffTime - Time.deltaTime;
        }

        if (buffTime <= 0)
        {
            obiRope.enabled = false;
            startPoint.target = null;
            endPoint.target = null;
            cutting = true;
        }
        
    }
}

