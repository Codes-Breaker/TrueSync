using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public struct cruiserData
{
    public Vector3 position;
    public Vector3 quaternion;
}

public class MissilBoomSkill : SkillItemBase
{
    public GameObject cruiserPrefab;
    public GameObject missileBoomPrefab;
    [Header("巡航舰升起时间")]
    public float cruiserRiseTime;
    [Header("导弹飞行时间")]
    public float missileBoomDurationTime;

    [Header("导弹飞行高度")]
    public float missileBoomHeight = 2f;
    [Header("爆炸范围")]
    public float explosionRangeRadius;
    [Header("爆炸力")]
    public float explosionForceArgument;


    private List<cruiserData> cruiserDatas = new List<cruiserData>();

    private GameObject cruiserGameObject;
    private GameObject missileBoomGameObject;
    private CinemachineTargetGroup cinemachineTargetGroup;
    private string missileBombExplosionPath = "Prefabs/Effect/MissileBomb_Explosion";

    private float currentTIme;

    private void AddCruiserData()
    {
        cruiserDatas.Add(new cruiserData
        {
            position = new Vector3(-15.44f, 0.82f, 8.43f),
            quaternion = new Vector3(0, 37.152f, 0),
        });
        cruiserDatas.Add(new cruiserData
        {
            position = new Vector3(-18.11f, 1f, -8.76f),
            quaternion = new Vector3(0, -31.544f, 0),
        });
        cruiserDatas.Add(new cruiserData
        {
            position = new Vector3(15.2f, 1f, 8.95f),
            quaternion = new Vector3(0, -31.544f, 0),
        });
        cruiserDatas.Add(new cruiserData
        {
            position = new Vector3(15.92f, 1f, -8.47f),
            quaternion = new Vector3(0, 37.677f, 0),
        });
    }

    public override void Init(SkillItemCreatData skillItemCreatData)
    {
        base.Init(skillItemCreatData);
        AddCruiserData();
        cinemachineTargetGroup = GameObject.FindObjectOfType<CinemachineTargetGroup>();
    }
    public override void Show()
    {
        base.Show();
        var cruiserData = cruiserDatas[Random.Range(0, cruiserDatas.Count)];
        cruiserGameObject = Instantiate(cruiserPrefab, new Vector3(cruiserData.position.x,-1,cruiserData.position.z),Quaternion.Euler(cruiserData.quaternion),transform);
        cinemachineTargetGroup.AddMember(cruiserGameObject.transform, 2, 4);
        cruiserGameObject.transform.DOMoveY(cruiserData.position.y, cruiserRiseTime).OnComplete(()=>{
            LaunchMissileBoom();
            cruiserGameObject.transform.DOMoveY(-2f, cruiserRiseTime).OnComplete(() => {
                cinemachineTargetGroup.RemoveMember(cruiserGameObject.transform);
                Destroy(cruiserGameObject);
            });
        });
    }
    private void LaunchMissileBoom()
    {
        missileBoomGameObject = Instantiate(missileBoomPrefab,transform);
        missileBoomGameObject.transform.position = cruiserGameObject.transform.position;
        Vector3 lookAtTarget = missileBoomGameObject.transform.position - missileBoomGameObject.transform.forward;
        missileBoomGameObject.transform.DOPath(new Vector3[] { cruiserGameObject.transform.position, (cruiserGameObject.transform.position + skillItemData.targetPosition) / 2 + Vector3.up * missileBoomHeight, skillItemData.targetPosition }, missileBoomDurationTime, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .OnWaypointChange((int wayPointIndex)=>{
                if (wayPointIndex > 0)
                    lookAtTarget = missileBoomGameObject.transform.position + missileBoomGameObject.transform.forward;
                else
                    lookAtTarget = missileBoomGameObject.transform.position + missileBoomGameObject.transform.forward;
            })
            .OnUpdate(()=>missileBoomGameObject.transform.LookAt(lookAtTarget))
            .OnComplete(() => {
                MissileBoomExplode();
                Destroy(missileBoomGameObject);
                Destroy(gameObject,3f);
            });
    }

    private void MissileBoomExplode()
    {
        var colliders = FindObjectsOfType<Collider>();
        if (colliders.Length != 0)
        {
            foreach (var item in colliders)
            {
                if ((item.transform.position - skillItemData.targetPosition).magnitude < explosionRangeRadius)
                {
                    if (item.GetComponent<CharacterContorl>() && !item.GetComponent<CharacterContorl>().invulernable)
                    {
                        item.GetComponent<Rigidbody>().AddExplosionForce(explosionForceArgument, skillItemData.targetPosition, explosionRangeRadius);
                    }

                    if(item.GetComponent<IRandomEventsObject>() != null)
                    {
                        item.GetComponent<IRandomEventsObject>().OnExit();
                    }
                }
            }
        }
        var effectPrefab = Resources.Load(missileBombExplosionPath);
        var effectGameObject = (GameObject)Instantiate(effectPrefab, skillItemData.targetPosition, Quaternion.Euler(Vector3.zero),transform);
        effectGameObject.transform.localScale = explosionRangeRadius* Vector3.one/4;

    }

}
