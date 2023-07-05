using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotMarker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        var childCount = this.transform.childCount;
        for(int i = 0; i < childCount; i++)
        {
            Gizmos.DrawSphere(this.transform.GetChild(i).position, 2);
        }
    }
}
