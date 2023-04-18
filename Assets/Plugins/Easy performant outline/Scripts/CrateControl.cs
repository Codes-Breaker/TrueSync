using EPOOutline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public List<Outlinable> outlinables;

    // Update is called once per frame
    void Update()
    {
        foreach(var outline in outlinables)
        {
            var currentCoef = Mathf.Abs(Mathf.Sin(Time.time));
            outline.OutlineParameters.DilateShift = currentCoef;
        }
    }
}
