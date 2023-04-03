using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SubmarineControl : MonoBehaviour,IRandomEventsObject
{
    public float stayTime;
    private float currentTime;
    private bool isShow;
    public float startTime;
    public float endTime;
    public float prepareShowTime;
    private bool startPrepare;
    private bool showComplete = false;
    public Material mat1; //��ͨ
    public Material mat2; //Ԥ����
    public Collider meshCollider;

    public List<(Vector3, Vector3)> randomPlaceAndRotation = new List<(Vector3, Vector3)>()
    {
        (new Vector3(6.85f, 2.55f, -0.92f), new Vector3(0, -48, 0)),
        (new Vector3(-7.71f, 2.55f, -2.65f), new Vector3(0, 48, 0)),
        (new Vector3(-7.71f, 2.55f, 2.65f), new Vector3(0, -48, 0)),
        (new Vector3(-7.71f, 2.55f, 2.65f), new Vector3(0, -48, 0)),
        (new Vector3(6.85f, 2.55f, 2.65f), new Vector3(0, 48, 0)),
    };

    private void Update()
    {
        if (!isShow)
            return;
        if (isShow && !showComplete)
        {
            showComplete = true;
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            transform.gameObject.GetComponent<MeshRenderer>().material = mat1;
            GetComponent<Collider>().enabled = true;
            transform.DOLocalMoveY(2.3f, startTime).OnComplete(() => {
                isShow = true;
            });
        }
        currentTime += Time.deltaTime;
        if(currentTime > stayTime)
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
        transform.DOLocalMoveY(0f, endTime).OnComplete(() => {
            this.gameObject.SetActive(false);
            isShow = false;
            startPrepare = false;
            showComplete = false;
        });
    }

    public void OnShow(Vector3 position)
    {
        var random = randomPlaceAndRotation[Random.Range(0, randomPlaceAndRotation.Count)];
        transform.position = random.Item1;
        transform.rotation = Quaternion.Euler(random.Item2);
        GetComponent<Collider>().enabled = false;
        startPrepare = true;
        transform.position = new Vector3(transform.position.x, 2.3f, transform.position.z);
        transform.gameObject.GetComponent<MeshRenderer>().material = mat2;
        currentTime = 0;
        this.gameObject.SetActive(true);

    }
}
