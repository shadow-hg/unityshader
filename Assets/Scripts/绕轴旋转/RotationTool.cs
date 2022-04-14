using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class RotationTool : MonoBehaviour
{
    [Header("需要旋转的物体")]
    public GameObject MainMesh;//需要旋转的物体
    
    public enum Axis
    {
        x = 0,
        y = 1,
        z = 2
    }
    [Header("旋转轴")]
    public Axis axis;

    [Header("旋转速度(负数时反方向旋转)")]
    public float Speed = 1.0f;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRotate();
    }

    void OnGUI()
    {
        //axis = (Axis)EditorGUILayout.EnumPopup("旋转轴:", axis);
    }

    void UpdateRotate()
    {
        switch (axis)
        {
            case Axis.x:
                MainMesh.transform.Rotate(Vector3.right * Time.deltaTime * Speed);
                break;
            case Axis.y:
                MainMesh.transform.Rotate(Vector3.up * Time.deltaTime * Speed);
                break;
            case Axis.z:
                MainMesh.transform.Rotate(Vector3.forward * Time.deltaTime * Speed);
                break;
        }
    }
}
