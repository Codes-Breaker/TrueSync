using MudBun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public MudTorus Puff;

    public float Period = 1.0f;

    public float BounceHeight = 2.0f;

    public float MaxPuffRadius = 3.0f;
    public float MaxPuffSize = 1.0f;

    private float m_timer = 0.0f;
    // Update is called once per frame
    void Update()
    {
        m_timer += Time.deltaTime;
        m_timer = Mathf.Repeat(m_timer, Period);

        float t = m_timer / Period;

        float s = 2.0f * (t - 0.5f);

        Puff.transform.localScale = new Vector3(MaxPuffRadius * (t + 0.2f) / 1.2f, 1.0f, MaxPuffRadius * (t + 0.2f) / 1.2f);

        if (t < 0.5f)
        {
            Puff.Radius = MaxPuffSize * ((t + 0.4f) / 0.9f);
        }
        else
        {
            Puff.Radius = MaxPuffSize * (1.0f - t) * 2.0f;
        }
    }
}
