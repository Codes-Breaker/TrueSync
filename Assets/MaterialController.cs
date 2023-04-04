using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Material> normalMaterial;
    public List<Material> transparentMaterial;
    public Renderer skinnedMeshRenderer;

    void Start()
    {
        
    }

    public void SetFlashMeshRendererBlock(bool value)
    {
        if (value)
        {
            var tempMats = skinnedMeshRenderer.materials;
            for(int i = 0; i < transparentMaterial.Count; i++)
            {
                tempMats[i] = transparentMaterial[i];
            }
            skinnedMeshRenderer.materials = tempMats;
        }
        else
        {
            var tempMats = skinnedMeshRenderer.materials;
            for (int i = 0; i < normalMaterial.Count; i++)
            {
                tempMats[i] = normalMaterial[i];
            }
            skinnedMeshRenderer.materials = tempMats;
        }

        for(int i = 0; i < normalMaterial.Count; i++)
        {
            var rendererBlock = new MaterialPropertyBlock();
            skinnedMeshRenderer.GetPropertyBlock(rendererBlock, i);
            rendererBlock.SetFloat("_PlayHurt", value ? 1f : 0f);
            skinnedMeshRenderer.SetPropertyBlock(rendererBlock, i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
