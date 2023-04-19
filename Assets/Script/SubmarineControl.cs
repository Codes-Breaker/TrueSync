using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SubmarineControl : MonoBehaviour, IRandomEventsObject
{
    public float stayTime;
    private float currentTime;
    private bool isShow;
    private float startTime;
    public float endTime;
    public float prepareShowTime;
    private bool startPrepare;
    private bool showComplete = false;
    public Material mat1; //��ͨ
    public Material mat2; //Ԥ����
    public Collider meshCollider;
    public MeshRenderer meshRenderer;
    public GameObject hitEffect;
    public int randomIndex = -1;

    public List<(Vector3, Vector3)> randomPlaceAndRotation = new List<(Vector3, Vector3)>()
    {
        (new Vector3(6.85f, 2.55f, -0.92f), new Vector3(0, -48, 0)), //左上
        (new Vector3(-7.71f, 2.55f, -2.65f), new Vector3(0, 48, 0)), //右上
        (new Vector3(-7.71f, 2.55f, 2.65f), new Vector3(0, -48, 0)), //右下
        (new Vector3(6.85f, 2.55f, 2.65f), new Vector3(0, 48, 0)), //左下
        (new Vector3(-0.26f, 0, 2.65f), new Vector3(0, 0, 0)), //中
    };

    private void Update()
    {
        if (!isShow)
            return;
        if (isShow && !showComplete)
        {
            showComplete = true;
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            meshRenderer.material = mat1;
            meshCollider.enabled = true;
            transform.DOLocalMoveY(3.2f, 2).OnComplete(() =>
            {
                isShow = true;
            });
        }
        currentTime += Time.deltaTime;
        if (currentTime > stayTime)
        {
            OnExit();
        }
    }

    private void FixedUpdate()
    {
        if (startPrepare)
        {
            prepareShowTime = prepareShowTime -= Time.fixedDeltaTime;
            if (prepareShowTime <= 0)
            {
                isShow = true;
            }
        }
    }

    public void OnExit()
    {
        if(currentTime< stayTime)
        {
            hitEffect.SetActive(true);
        }
        transform.DOLocalMoveY(0f, 2).OnComplete(() =>
        {
            this.gameObject.SetActive(false);
            isShow = false;
            startPrepare = false;
            showComplete = false;
        });
    }

    public void OnShow(Vector3 position, float stayTime)
    {
        this.stayTime = stayTime;
        randomIndex = Random.Range(0, randomPlaceAndRotation.Count);
        var rand = randomPlaceAndRotation[randomIndex];
        position = rand.Item1;
        transform.rotation = Quaternion.Euler(rand.Item2);
        startPrepare = true;
        transform.position = position;
        meshCollider.enabled = false;
        meshRenderer.material = mat2;
        transform.position = new Vector3(transform.position.x, 3.2f, transform.position.z);
        currentTime = 0;
        hitEffect.SetActive(false);
        this.gameObject.SetActive(true);

    }
}
