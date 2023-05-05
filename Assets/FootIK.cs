using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FootIK : MonoBehaviour
{
    public Transform footTransformRF;
    public Transform footTransformRB;
    public Transform footTransformLF;
    public Transform footTransformLB;

    private Transform[] allFootTransforms;

    public Transform targetTransformRF;
    public Transform targetTransformRB;
    public Transform targetTransformLF;
    public Transform targetTransformLB;

    private Transform[] allTargetTransforms;

    public GameObject footRigRF;
    public GameObject footRigRB;
    public GameObject footRigLB;
    public GameObject footRigLF;

    private TwoBoneIKConstraint[] allFootIKContrains;
    private LayerMask groundLayerMask;
    private LayerMask hitLayer;
    public float maxHitDistance;
    public float addedHeight;
    private bool[] allGroundSpherecastHits;
    private Vector3[] allHitNormals;
    private float angleAboutX;
    private float angleAboutZ;
    public float yOffset = 0.15f;

    // Start is called before the first frame update
    void Start()
    {
        allFootTransforms = new Transform[4];

        allFootTransforms[0] = footTransformRF;
        allFootTransforms[1] = footTransformRB;
        allFootTransforms[2] = footTransformLF;
        allFootTransforms[3] = footTransformLB;

        allTargetTransforms = new Transform[4];

        allTargetTransforms[0] = targetTransformRF;
        allTargetTransforms[1] = targetTransformRB;
        allTargetTransforms[2] = targetTransformLF;
        allTargetTransforms[3] = targetTransformLB;

        allFootIKContrains = new TwoBoneIKConstraint[4];
        allFootIKContrains[0] = footRigRF.GetComponent<TwoBoneIKConstraint>();
        allFootIKContrains[1] = footRigRB.GetComponent<TwoBoneIKConstraint>();
        allFootIKContrains[2] = footRigLB.GetComponent<TwoBoneIKConstraint>();
        allFootIKContrains[3] = footRigLF.GetComponent<TwoBoneIKConstraint>();

        allGroundSpherecastHits = new bool[5];

        allHitNormals = new Vector3[4];

        groundLayerMask = LayerMask.NameToLayer("Floor");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        RotateCharacterFeet();
    }

    private void CheckGroundBelow(out Vector3 hitPoint, out bool gotGroundSphereCastHit, out Vector3 hitNormal, out LayerMask hitLayer, 
        out float currentHitDistance, Transform objectTransform, int checkForLayerMask, float maxHitDistance, float addedHeight)
    {
        RaycastHit hit;
        Vector3 startSpherecast = objectTransform.position + new Vector3(0f, addedHeight, 0f);
        if (checkForLayerMask == -1)
        {
            Debug.LogError("Layer does not exist!");
            gotGroundSphereCastHit = false;
            currentHitDistance = 0f;
            hitLayer = LayerMask.NameToLayer("Player");
            hitNormal = Vector3.up;
            hitPoint = objectTransform.position;
        }
        else
        {
            int layerMask = (1 << checkForLayerMask);
            if (Physics.SphereCast(startSpherecast, .2f, Vector3.down, out hit, maxHitDistance, layerMask, QueryTriggerInteraction.UseGlobal))
            {
                hitLayer = hit.transform.gameObject.layer;
                currentHitDistance = hit.distance - addedHeight;
                hitNormal = hit.normal;
                gotGroundSphereCastHit = true;
                hitPoint = hit.point;
            }
            else
            {
                gotGroundSphereCastHit = false;
                currentHitDistance = 0f;
                hitLayer = LayerMask.NameToLayer("Player");
                hitNormal = Vector3.up;
                hitPoint = objectTransform.position;
            }
        }
    }

    Vector3 ProjectOnContactPlane(Vector3 vector, Vector3 hitNormal)
    {
        return vector - hitNormal * Vector3.Dot(vector, hitNormal);
    }
    
    private void ProjectedAxisAngles(out float angleAboutX, out float angleAboutZ, Transform footTargetTransform, Vector3 hitNormal)
    {
        Vector3 xAxisProject = ProjectOnContactPlane(footTargetTransform.forward, hitNormal).normalized;
        Vector3 zAxisProject = ProjectOnContactPlane(footTargetTransform.right, hitNormal).normalized;

        angleAboutX = Vector3.SignedAngle(footTargetTransform.forward, xAxisProject, footTargetTransform.right);
        angleAboutZ = Vector3.SignedAngle(footTargetTransform.right, zAxisProject, footTargetTransform.forward);
    }

    private void RotateCharacterFeet()
    {
        for (int i = 0; i < 4; i++)
        {
            CheckGroundBelow(out Vector3 hitPoint, out allGroundSpherecastHits[i], out Vector3 hitNormal, out hitLayer, out _,
                allFootTransforms[i], groundLayerMask, maxHitDistance, addedHeight);
            allHitNormals[i] = hitNormal;

            if (allGroundSpherecastHits[i])
            {
                ProjectedAxisAngles(out angleAboutX, out angleAboutZ, allFootTransforms[i], allHitNormals[i]);
                allTargetTransforms[i].position = new Vector3(allFootTransforms[i].position.x, hitPoint.y + yOffset, allFootTransforms[i].position.z);
                allTargetTransforms[i].rotation = allFootTransforms[i].rotation;
            }
            else
            {
                allTargetTransforms[i].position = allFootTransforms[i].position;
                allTargetTransforms[i].rotation = allFootTransforms[i].rotation;
            }
        }
    }
}
