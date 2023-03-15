using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEffect : MonoBehaviour
{
    private float[] speeds = new float[4];
    private float[] times = new float[4];
    private Vector3[] localPositions = new Vector3[4];
    private float[] scales = new float[4];
    private Renderer[] _renderers;
    private List<MaterialPropertyBlock> _propBlocks = new List<MaterialPropertyBlock>();
    private float radius = 1;


    // Start is called before the first frame update
    void Start()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        foreach (var _ in _renderers)
        {
            _propBlocks.Add(new MaterialPropertyBlock());
        }
    }

    public void SetRadius(float r)
    {
        radius = r;
    }

    public void AddCollision(Vector3 worldPosition, float time, float scale = 1)
    {
        if (time <= 0)
            return;
        int index = -1;
        for (int i = 0; i < times.Length; ++i)
        {
            if (times[i] == 0)
            {
                index = i;
                break;
            }
        }
        if (index != -1)
        {
            times[index] = time;
            speeds[index] = 1;
            localPositions[index] = (worldPosition - transform.position).normalized;
            scales[index] = scale;
        }
    }

    private float EaseOutCirc(float x)
    {
        return Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));
    }

    private float EaseOutQuint(float x)
    {
        return 1 - Mathf.Pow(1 - x, 5);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < times.Length; ++i)
        {
            if (times[i] == 0)
                continue;
            Vector3 worldPosition = Vector3.zero;
            Vector3 localPosition = Vector3.zero;
            float scale = 0;
            if (times[i] > 0)
            {
                localPosition = localPositions[i] * radius;
                worldPosition = localPosition + transform.position;
                times[i] = times[i] - Time.deltaTime;
                if (times[i] <= 0)
                {
                    times[i] = 0;
                    scale = EaseOutCirc(times[i] * speeds[i]) * scales[i];
                    for (int j = 0; j < _renderers.Length; ++j)
                    {
                        var _renderer = _renderers[j];
                        var _propBlock = _propBlocks[j];
                        _renderer.GetPropertyBlock(_propBlock);
                        _propBlock.SetVector($"collisionpoint_{i}", worldPosition);
                        _propBlock.SetVector($"collisionvector_{i}", localPosition);
                        _propBlock.SetFloat($"collisionvectorscale_{i}", Mathf.Clamp(scale, 0, radius));
                        _renderer.SetPropertyBlock(_propBlock);
                    }
                    break;
                }
                else
                    scale = EaseOutCirc(times[i] * speeds[i]) * scales[i];
            }
            for (int j = 0; j < _renderers.Length; ++j)
            {
                var _renderer = _renderers[j];
                var _propBlock = _propBlocks[j];
                _renderer.GetPropertyBlock(_propBlock);
                _propBlock.SetVector($"collisionpoint_{i}", worldPosition);
                _propBlock.SetVector($"collisionvector_{i}", localPosition);
                _propBlock.SetFloat($"collisionvectorscale_{i}", Mathf.Clamp(scale, 0, radius));
                _renderer.SetPropertyBlock(_propBlock);
            }
        }
    }
}