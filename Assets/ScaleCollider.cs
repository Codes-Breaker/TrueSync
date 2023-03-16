using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleCollider : MonoBehaviour
{
    public SkinnedMeshRenderer renderer;
    public List<SkinnedMeshRenderer> otherRenderers;

    public SphereCollider scollider;

    [Range(0, 1)]
    public float scale;

    public Vector3 targetPosition;
    public float targetRadius;

    private Vector3 originalPosition;
    private float originalRadius;

    public CharacterJoint leftHand;
    public CharacterJoint rightHand;

    private Vector3 originalLeftPos;
    private Vector3 originalRightPos;

    public Vector3 targetLeftPos;
    public Vector3 targetRightPos;


    // Start is called before the first frame update
    void Start()
    {
        originalPosition = scollider.center;
        originalRadius = scollider.radius;

        originalLeftPos = leftHand.anchor;
        originalRightPos = rightHand.anchor;
    }

    public void SetScale(float gasScale)
    {
        scale = gasScale;
    }
        
    void Update()
    {
        scollider.center = Vector3.Lerp(originalPosition, targetPosition, scale);
        scollider.radius = Mathf.Lerp(originalRadius, targetRadius, scale);

        leftHand.anchor = Vector3.Lerp(originalLeftPos, targetLeftPos, scale);
        rightHand.anchor = Vector3.Lerp(originalRightPos, targetRightPos, scale);

        renderer.SetBlendShapeWeight(0, scale*100);
        if (otherRenderers.Count != 0)
        {
            foreach (var item in otherRenderers)
            {
                if (item.gameObject.activeSelf)
                    item.SetBlendShapeWeight(0, scale * 100);
            }
        }


    }
}
