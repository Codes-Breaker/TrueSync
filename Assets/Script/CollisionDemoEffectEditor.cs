using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CollisionDemoEffect))]
public class CollisionEffectDemoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("�����ײ��"))
        {
            ((CollisionDemoEffect)(target)).AddCollision();
        }
    }
}
