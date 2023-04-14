using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBoomSkillController : SkillItemControllerBase
{
    public LayerMask layerMask; //地面的Layer
    public float skillMarkerSize = 5;//目标大小

    private RaycastHit hitInfo;
    private Vector3 currentTargetPoint;

    public GameObject skillAreaMarker;

    public GameObject MissilBoomPrefab;

    public List<(Vector3, Vector3)> randomPlaceAndRotation = new List<(Vector3, Vector3)>()
    {
        (new Vector3(11.37f, 2f, 5.6f), new Vector3(0, -48, 0)),
        (new Vector3(11.37f, 2f, -5.6f), new Vector3(0, 48, 0)),
        (new Vector3(0.33f, 2f, 0.58f), new Vector3(0, -48, 0)),
        (new Vector3(-11.64f, 2f, -5.6f), new Vector3(0, -48, 0)),
        (new Vector3(-11.64f, 2f, 5.6f), new Vector3(0, 48, 0)),
    };

    protected override void Init()
    {
        base.Init();
        skillAreaMarker.SetActive(false);
    }

   

    public override void UseSkillItem()
    {
        base.UseSkillItem();
        SetControlMissileBoom();
    }

    private void SetControlMissileBoom()
    {
        user.moveAciotn = MoveAim;
        user.interactWeaponAction = LaunchMissile;
        user.chargeAction = null;
    }

    private void MoveAim(Vector2 axisInput, ControlDeviceType controlDeviceType)
    {
        //鼠标控制方案
        if(controlDeviceType == ControlDeviceType.Mouse)
        {
            if (!skillAreaMarker.activeSelf)
            {
                skillAreaMarker.SetActive(true);
                var point = Camera.main.WorldToScreenPoint(user.transform.position);
                Ray ray = Camera.main.ScreenPointToRay(point);
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask))
                {
                    skillAreaMarker.transform.position = hitInfo.point;
                    skillAreaMarker.transform.localScale = Vector3.one * skillMarkerSize;
                    currentTargetPoint = skillAreaMarker.transform.position;
                }
                else
                {
                    skillAreaMarker.SetActive(false);
                }
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(axisInput); 

                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask))
                {
                    skillAreaMarker.transform.position = Vector3.Lerp(skillAreaMarker.transform.position, hitInfo.point, 0.1f);
                    skillAreaMarker.transform.localScale = Vector3.one * skillMarkerSize;
                    currentTargetPoint = skillAreaMarker.transform.position;
                }
                else
                {
                    skillAreaMarker.SetActive(false);
                }
            }
        }
        else
        {
            if (!skillAreaMarker.activeSelf)
            {
                skillAreaMarker.SetActive(true);
                var point = Camera.main.WorldToScreenPoint(user.transform.position);
                Ray ray = Camera.main.ScreenPointToRay(point);
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask))
                {
                    skillAreaMarker.transform.localPosition = transform.InverseTransformPoint(hitInfo.point);
                    skillAreaMarker.transform.localScale = Vector3.one * skillMarkerSize;
                    currentTargetPoint = skillAreaMarker.transform.position;
                }
                else
                {
                    skillAreaMarker.SetActive(false);
                }

            }
            else
            {
                var delta = axisInput.normalized;
                Vector3 targetPoint = skillAreaMarker.transform.position - new Vector3(delta.x, 0, delta.y);

                var point = Camera.main.WorldToScreenPoint(targetPoint);
                Ray ray = Camera.main.ScreenPointToRay(point);
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask))
                {
                    skillAreaMarker.transform.localPosition = Vector3.Lerp(skillAreaMarker.transform.localPosition, transform.InverseTransformPoint(hitInfo.point), 0.3f);
                    skillAreaMarker.transform.localScale = Vector3.one * skillMarkerSize;
                    currentTargetPoint = skillAreaMarker.transform.position;
                }
                else
                {
                    skillAreaMarker.SetActive(false);
                }
            }

        }
    }

    public override void ExitUseMode()
    {
        base.ExitUseMode();
        skillAreaMarker.SetActive(false);
    }
    private void LaunchMissile(bool isButtonUp)
    {
        if (!isButtonUp)
        {
            //发射导弹
            var missilBoom = Instantiate(MissilBoomPrefab);
            if (missilBoom.GetComponent<SkillItemBase>())
            {
                missilBoom.GetComponent<SkillItemBase>().Init(new SkillItemCreatData
                {
                    targetPosition = currentTargetPoint
                });
                missilBoom.GetComponent<SkillItemBase>().Show();
            }
           //控制器重设，销毁自身
            user.SetControlSelf();

            OnEnd();
        }
    }

    protected override void OnEnd()
    {
        base.OnEnd();
        skillAreaMarker.SetActive(false);
    }
}
