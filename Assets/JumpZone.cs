using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpZone : MonoBehaviour
{
    public Collider platformCollider;
    private void OnTriggerEnter(Collider other)
    {
        var character = other.gameObject.GetComponent<CharacterContorl>();
        if (character != null && !character.jumpingBack)
        {
            character.returning = false;
            character.jumpingBack = true;
            Vector3 pos = Physics.ClosestPoint(character.transform.position, platformCollider, platformCollider.transform.position, platformCollider.transform.rotation);
            character.jumpTarget = new Vector3(pos.x, platformCollider.transform.position.y + 1, pos.z);
            character.SetUpJump();
        }
    }
}
