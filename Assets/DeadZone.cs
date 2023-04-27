using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public Collider platformCollider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //var character = other.gameObject.GetComponent<CharacterContorl>();
        //if (character != null && !character.returning)
        //{
        //    character.SetUpReturn();
        //    character.returning = true;
        //    character.jumpingBack = false;
        //    Vector3 pos = Physics.ClosestPoint(character.transform.position, platformCollider, platformCollider.transform.position, platformCollider.transform.rotation);
        //    pos = new Vector3(pos.x, character.transform.position.y, pos.z);
        //    character.swimTarget = pos;
        //}
    }
}
