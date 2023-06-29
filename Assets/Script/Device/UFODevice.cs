using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFODevice : MonoBehaviour
{
    [Header("向上吸力参数")]
    public AnimationCurve suctionToTargetForceParameters;

    [Header("水平吸力参数")]
    public AnimationCurve suctionToCenterForceParameters;

    [Header("模拟空气阻力")]
    public float frictionCoefficient = 0.6f;

    [Header("吸力终点")]
    public Transform suctionTarget;

    [Header("是否有吸力")]
    public bool isSuction;

    public bool active;

    private bool hasAddGroup = false;
    private bool hasRemoveGroup = false;
    public int catchedPlayer => catchedPlayers.Count;

    public List<CharacterContorl> catchedPlayers = new List<CharacterContorl>();
    private void LateUpdate()
    {
        if (active)
        {
            if (!hasAddGroup)
            {
                AddCamGroup();
            }
        }
        else
        {
            if (!hasRemoveGroup)
            {
                RemoveCamGroup();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(isSuction)
        {

            var otherRB = other.GetComponent<Rigidbody>();
            if (otherRB)
            {
                var otherPositon =  other.transform.position;
                var selfPositon = suctionTarget.position;
                var distance = Mathf.Abs(otherPositon.y - selfPositon.y);
                var hdistance = (otherPositon - selfPositon).magnitude;
                var target = (selfPositon - otherPositon).normalized;



                //otherRB.AddForce(Vector3.up * suctionToTargetForceParameters.Evaluate(distance) * otherRB.mass, ForceMode.Force);
                var hForce = suctionToCenterForceParameters.Evaluate(distance);
                otherRB.velocity = new Vector3(otherRB.velocity.x, 0.9f,otherRB.velocity.z);
                otherRB.AddForce(Vector3.ProjectOnPlane(target, Vector3.up).normalized * hForce * otherRB.mass, ForceMode.Force);
                // 计算空气阻力
                Vector3 airResistance = - otherRB.velocity * frictionCoefficient;
                // 应用空气阻力
                otherRB.AddForce(Vector3.ProjectOnPlane(airResistance , Vector3.up) * otherRB.mass);
                var character = otherRB.GetComponent<CharacterContorl>();
                if (character)
                {
                    character.isAirWalk = true;
                    if (hdistance <= 1 && !catchedPlayers.Contains(character))
                    {
                        var immuneBuff = new UFODamageImmuneBuff(character, -1);
                        character.OnGainBuff(immuneBuff);
                        catchedPlayers.Add(character);
                    }
                }
                Debug.Log($"distance : {distance} hdistance : {hdistance} hforce {hForce}");
            }
        }
        else
        {

            var otherRB = other.GetComponent<Rigidbody>();
            if (otherRB)
            {
                var character = otherRB.GetComponent<CharacterContorl>();
                if (character)
                {
                    character.FinishUFOImmuneBuff();
                    character.isAirWalk = false;
                    if (catchedPlayers.Contains(character))
                        catchedPlayers.Remove(character);
                }
            }
        }
    }

    private void AddCamGroup()
    {
        hasAddGroup = true;
        hasRemoveGroup = false;

        var groups = GameObject.FindObjectsOfType<CinemachineTargetGroup>();
        foreach(var g in groups)
        {
            g.AddMember(this.transform.parent, 2, 12);
        }
    }

    private void RemoveCamGroup()
    {
        hasRemoveGroup = true;
        var groups = GameObject.FindObjectsOfType<CinemachineTargetGroup>();
        foreach (var g in groups)
        {
            g.RemoveMember(this.transform.parent);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var otherRB = other.GetComponent<Rigidbody>();
        if (otherRB)
        {
            var character = otherRB.GetComponent<CharacterContorl>();
            if (character)
            {
                character.isAirWalk = false;
            }
        }
    }

}
