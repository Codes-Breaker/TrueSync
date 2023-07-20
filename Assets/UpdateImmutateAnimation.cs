using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateImmutateAnimation : MonoBehaviour
{
    public List<ImitateAnimation> immutates = new List<ImitateAnimation>();
    public bool immutate = true;
    public Transform originalRoot;
    public Transform resultRoot;
    private void OnAnimatorMove()
    {
        UpdatePositionRotationOfCopy(originalRoot, resultRoot);
        foreach (var im in immutates)
        {
            im.UpdatePosition();
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {

    }

    private void UpdatePositionRotationOfCopy(Transform originalRoot, Transform copyRoot)
    {
        copyRoot.transform.position = originalRoot.transform.position;
        copyRoot.transform.rotation = originalRoot.transform.rotation;
        if (copyRoot.childCount > 0)
        {
            for(int i = 0; i < copyRoot.childCount; i++)
            {
                UpdatePositionRotationOfCopy(originalRoot.GetChild(i), copyRoot.GetChild(i)); 
            }
        }
    }
}
