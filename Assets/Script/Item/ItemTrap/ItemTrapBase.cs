using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class ItemTrapBase : MonoBehaviour
{
    public CharacterContorl character;
    
    public void Init(CharacterContorl character)
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
