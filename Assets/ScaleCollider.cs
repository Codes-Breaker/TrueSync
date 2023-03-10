using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleCollider : MonoBehaviour
{
    public SkinnedMeshRenderer renderer;

    public SphereCollider scollider;

    [Range(0, 1)]
    public float scale;

    public Vector3 targetPosition;
    public float targetRadius;

    private Vector3 originalPosition;
    private float originalRadius;

    public CharacterJoint leftJoint;
    public CharacterJoint rightJoint;

    private Vector3 originalLeftJointAxis;
    private Vector3 originalRightJointAxis;

    public Vector3 targetLeftJointAxis;
    public Vector3 targetRightJointAxis;


    // Start is called before the first frame update
    void Start()
    {
        originalPosition = scollider.center;
        originalRadius = scollider.radius;

        originalLeftJointAxis = leftJoint.connectedAnchor;
        originalRightJointAxis = rightJoint.connectedAnchor;
    }

    // Update is called once per frame
    void Update()
    {
        scollider.center = Vector3.Lerp(originalPosition, targetPosition, scale);
        scollider.radius = Mathf.Lerp(originalRadius, targetRadius, scale);

        leftJoint.connectedAnchor = Vector3.Lerp(originalLeftJointAxis, targetLeftJointAxis, scale);
        rightJoint.connectedAnchor = Vector3.Lerp(originalRightJointAxis, targetRightJointAxis, scale);

        renderer.SetBlendShapeWeight(0, scale*100);


    }
}
