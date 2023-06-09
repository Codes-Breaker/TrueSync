using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class ItemTrapBase : MonoBehaviour
{
    public CharacterContorl character;
    
    public virtual void Init(CharacterContorl character)
    {
        this.character = character;
        gameObject.GetComponent<Collider>().isTrigger = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        
    }

    public virtual void OnEnd()
    {
        Destroy(gameObject);
    }

}
