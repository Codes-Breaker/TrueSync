using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FixedScale : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var parent = this.transform.parent;
        this.transform.parent = null;
        this.transform.localScale = Vector3.one;
        this.transform.parent = parent;
    }
}
