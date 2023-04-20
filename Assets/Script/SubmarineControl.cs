using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class SubmarineControl : MonoBehaviour, IRandomEventsObject
{
    public float stayTime;
    private float currentTime;
    private bool isShow;
    private float startTime;
    public float endTime;
    //public float prepareShowTime;
    //private bool startPrepare;
    //private bool showComplete = false;
    //public Material mat1; //��ͨ
    //public Material mat2; //Ԥ����
    //public Collider meshCollider;
    public int explosiveForce = 2000;
   // public MeshRenderer meshRenderer;
    public GameObject hitEffect;
    public int randomIndex = -1;

    public GameObject timeLapseBombPrefab;
    public float launchBomBradius = 6;
  //  public int launchBombNum = 2;
    private bool hasLaunch;

    //public List<(Vector3, Vector3)> randomPlaceAndRotation = new List<(Vector3, Vector3)>()
    //{
    //    (new Vector3(6.85f, 2.55f, -0.92f), new Vector3(0, -48, 0)), //左上
    //    (new Vector3(-7.71f, 2.55f, -2.65f), new Vector3(0, 48, 0)), //右上
    //    (new Vector3(-7.71f, 2.55f, 2.65f), new Vector3(0, -48, 0)), //右下
    //    (new Vector3(6.85f, 2.55f, 2.65f), new Vector3(0, 48, 0)), //左下
    //    (new Vector3(-0.26f, 0, 2.65f), new Vector3(0, 0, 0)), //中
    //};
    public struct SubmarinePathData{
        public Vector3 startPoint;
        public Vector3 endPoint;
        public Vector3 rotation;
    }

    public List<SubmarinePathData> submarinePathDatas = new List<SubmarinePathData>()
    {
        new SubmarinePathData
        {
            startPoint = new Vector3(0,0,11.35f),
            endPoint = new Vector3(0,0,-16.3f),
            rotation = new Vector3(0,0,0),
        },
        new SubmarinePathData
        {
            startPoint = new Vector3(0,0,-16.3f),
            endPoint = new Vector3(0,0,11.35f),
            rotation = new Vector3(0,180,0),
        },
        new SubmarinePathData
        {
            startPoint = new Vector3(8.37f, 0,11.5f),
            endPoint = new Vector3(-19.43f,0,-11.81f),
            rotation = new Vector3(0,50f,0)
        },
        new SubmarinePathData
        {
            startPoint = new Vector3(-18.34f, 0,9.51f),
            endPoint = new Vector3(24f,0,-11.4f),
            rotation = new Vector3(0,296.292f,0)
        },
        new SubmarinePathData
        {
            startPoint = new Vector3(20.4f, 0,0f),
            endPoint = new Vector3(-20f,0,0),
            rotation = new Vector3(0,90f,0)
        },
        new SubmarinePathData
        {
            startPoint = new Vector3(-20f, 0,0f),
            endPoint = new Vector3(20.4f, 0,0f),
            rotation = new Vector3(0,-90f,0)
        }
    };

    private void Update()
    {
        if (!isShow)
            return;
        currentTime += Time.deltaTime;
        if (currentTime > stayTime / 2 && !hasLaunch)
            launchBomb();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<IRandomEventsObject>() != null)
        {
            other.gameObject.GetComponent<IRandomEventsObject>().OnExit();
        }
        //if (other.gameObject.GetComponent<SkillItemControllerBase>())
        //    other.gameObject.GetComponent<SkillItemControllerBase>().OnEnd();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<IRandomEventsObject>() != null)
        {
            other.gameObject.GetComponent<IRandomEventsObject>().OnExit();
        }
        //if (other.gameObject.GetComponent<SkillItemControllerBase>())
        //    other.gameObject.GetComponent<SkillItemControllerBase>().OnEnd();
        if (other.gameObject.GetComponent<CharacterContorl>() && !other.gameObject.GetComponent<CharacterContorl>().invulernable)
        {
            BoxCollider trigger = GetComponent<BoxCollider>(); // 获取触发器的BoxCollider组件
            other.gameObject.GetComponent<CharacterContorl>().ridbody.AddExplosionForce(explosiveForce, other.contacts[0].point, 20);
        }
    }

    private void launchBomb()
    {
        hasLaunch = true;
        var character = FindObjectsOfType<CharacterContorl>();
        foreach(var item in character)
        {
            if(!item.isDead && !item.jumpingBack && !item.returning)
            {
                var bomb = Instantiate(timeLapseBombPrefab);
                Physics.IgnoreCollision(this.GetComponent<Collider>(), bomb.GetComponent<Collider>());
                bomb.transform.position = transform.position;
                bomb.SetActive(true);
                if (bomb.GetComponent<SkillItemBase>())
                {
                    var point = item.transform.position + Random.insideUnitSphere * launchBomBradius;
                    bomb.GetComponent<SkillItemBase>().Init(new SkillItemCreatData
                    {
                        targetPosition = new Vector3(point.x,item.transform.position.y,point.z)
                    }) ;
                    bomb.GetComponent<SkillItemBase>().Show();
                }

            }

        }
    }

    public void OnExit()
    {
        if(currentTime< stayTime)
        {
            hitEffect.SetActive(true);
            DOTween.Kill(transform);
        }
        var cinemachineTargetGroup = GameObject.FindObjectOfType<CinemachineTargetGroup>();
        cinemachineTargetGroup.RemoveMember(transform);
        transform.DOLocalMoveY(-5f, endTime).OnComplete(() =>
        {
            this.gameObject.SetActive(false);
            isShow = false;
        });
    }

    public void OnShow(Vector3 position, float stayTime)
    {
        this.stayTime = stayTime;
        randomIndex = Random.Range(0, submarinePathDatas.Count);
        var randPathData = submarinePathDatas[randomIndex];
        transform.rotation = Quaternion.Euler(randPathData.rotation);
       // startPrepare = true;
        transform.position =new Vector3( randPathData.startPoint.x,randPathData.startPoint.y-5f,randPathData.startPoint.z);
        //meshCollider.enabled = false;
        //meshRenderer.material = mat2;
        //transform.position = new Vector3(transform.position.x, 3.2f, transform.position.z);
        this.gameObject.SetActive(true);
        var cinemachineTargetGroup = GameObject.FindObjectOfType<CinemachineTargetGroup>();
        cinemachineTargetGroup.AddMember(transform, 2, 4);
        transform.DOMoveY(randPathData.startPoint.y, endTime).OnComplete(() =>
        { 
            transform.DOMove(randPathData.endPoint, stayTime)
                .OnComplete(() =>
                {
                    OnExit();
                })
                .SetEase(Ease.Linear);
            currentTime = 0;
            isShow = true;
        });
        hitEffect.SetActive(false);
        hasLaunch = false;
    }
}
