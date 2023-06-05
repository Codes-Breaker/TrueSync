using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterChooseControl : MonoBehaviour
{
    // Start is called before the first frame update

    public Camera characterCam;
    public SkinnedMeshRenderer meshRenderer;
    public float H;
    public float S;
    private float originalS;
    void Start()
    {
        var rendererBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(rendererBlock, 1);
        S = rendererBlock.GetFloat("_S");
        H = rendererBlock.GetFloat("_H");
        originalS = 0.51f;
        SetColor();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void SetColor()
    {
        var rendererBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(rendererBlock, 1);
        rendererBlock.SetFloat("_H", H);
        rendererBlock.SetFloat("_S", S);
        meshRenderer.SetPropertyBlock(rendererBlock, 1);
    }

    public void SwitchColor(bool left)
    {
        var x = left ? 0.1f : -0.1f;
        if (S == 0)
        {
            S = originalS;
            H += x;
        }
        else if (H + x > 1)
        {
            S = 0;
            H = 0;
        }
        else if (H + x < 0)
        {
            H = 1;
            S = 0;
        }
        else
        {
            H += x;
        }
        SetColor();
    }
}
