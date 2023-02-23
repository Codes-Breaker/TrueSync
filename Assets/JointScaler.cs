using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointScaler : MonoBehaviour
{
    private Vector3[] _connectedAnchor;
    private Vector3[] _anchor;
    public Transform[] affectedTransform;
    // Start is called before the first frame update
    void Start()
    {
        _connectedAnchor = new Vector3[affectedTransform.Length];
        _anchor = new Vector3[affectedTransform.Length];
        for (int i = 0; i < affectedTransform.Length; i++)
        {
            if (affectedTransform[i].GetComponent<Joint>() != null)
            {
                _connectedAnchor[i] = affectedTransform[i].GetComponent<Joint>().connectedAnchor;
                _anchor[i] = affectedTransform[i].GetComponent<Joint>().anchor;
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < affectedTransform.Length; i++)
     {
            if (affectedTransform[i].GetComponent<Joint>() != null)
         {
                affectedTransform[i].GetComponent<Joint>().connectedAnchor = _connectedAnchor[i];
                affectedTransform[i].GetComponent<Joint>().anchor = _anchor[i];
            }
        }
    }
}
