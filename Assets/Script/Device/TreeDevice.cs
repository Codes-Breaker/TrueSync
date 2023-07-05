using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TreeDevice : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        var character = collision.collider.GetComponent<CharacterContorl>();
        if(character)
        {
            if(character.buffs.Any(item => item is LargementPotionBuff))
            {
                if (!gameObject.GetComponent<Rigidbody>())
                {
                    var rb = gameObject.AddComponent<Rigidbody>();

                    rb.mass = 40;
                    gameObject.layer = 6;
                }
            }
        }
    }
}
