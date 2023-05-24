using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowGroundDetector : MonoBehaviour
{
    // Start is called before the first frame update

    public LayerMask GroundMask;
    public ParticleSystem particleEffect;
    public Transform targetTransform;

    private void LateUpdate()
    {
        this.transform.position = targetTransform.position;   
    }

    private void OnTriggerEnter(Collider other)
    {
        particleEffect.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        particleEffect.Stop();
    }

    void Start()
    {
        particleEffect.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
