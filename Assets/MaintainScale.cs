using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MaintainScale : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ScaleParent(this.transform);
    }

    private void ScaleParent(Transform transform)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (transform.localScale != Vector3.one)
            {
                child.localScale = new Vector3(child.localScale.x / transform.localScale.x, child.localScale.y / transform.localScale.y, child.localScale.z / transform.localScale.z);
            }
            ScaleParent(child);
        }

    }
}
