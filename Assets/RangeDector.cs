using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeDector : MonoBehaviour
{
    // Start is called before the first frame update

    public List<CharacterContorl> closeTargets;

    public CharacterContorl characterControl;

    void Start()
    {
        closeTargets = new List<CharacterContorl>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var rangeDector = other.GetComponent<RangeDector>();
        if (rangeDector != null && rangeDector.characterControl != null && !rangeDector.characterControl.isDead)
        {
            closeTargets.Add(rangeDector.characterControl);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var rangeDector = other.GetComponent<RangeDector>();
        if (rangeDector != null && rangeDector.characterControl != null)
        {
            closeTargets.Remove(rangeDector.characterControl);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
