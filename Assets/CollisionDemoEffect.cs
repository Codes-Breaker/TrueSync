using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDemoEffect : MonoBehaviour
{
    public List<float> angles = new List<float>();
    public List<float> times = new List<float>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCollision()
    {
        for (int i = 0; i < angles.Count; ++i)
        {
            var localPosition = new Vector3(Mathf.Cos(angles[i] * Mathf.Deg2Rad), 0, Mathf.Sin(angles[i] * Mathf.Deg2Rad));
            var worldPosition = localPosition + transform.position;
            GetComponent<CollisionEffect>().AddCollision(worldPosition, times[i]);
        }
    }
}
