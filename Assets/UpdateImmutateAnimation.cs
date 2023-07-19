using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateImmutateAnimation : MonoBehaviour
{
    public List<ImmutateAnimation> immutates = new List<ImmutateAnimation>();
    public bool immutate = true;
    private void OnAnimatorMove()
    {
        foreach(var im in immutates)
        {
            im.UpdatePosition();
        }
    }
}
