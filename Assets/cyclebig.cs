using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cyclebig : MonoBehaviour
{
    public SkinnedMeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        meshRenderer.SetBlendShapeWeight(0, Mathf.Abs(Mathf.Sin(Time.time)) * 100);
    }
}
