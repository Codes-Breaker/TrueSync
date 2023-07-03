using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class DuckInteractiveObject : InteractiveObject
{
    private Vector3 currentScale;

    private void Awake()
    {
        currentScale = this.transform.localScale;
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterContorl>() != null)
        {
            this.transform.DORewind();
            this.transform.DOShakeScale(0.5f, 1).SetEase(Ease.OutBounce);
        }

    }
}

