using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class InstanceEffect : MonoBehaviour
{
    public Camera mainCam;
    public bool reset = false;
    [Space(25)]
    public List<GameObject> effectsPrefabs;

    [Header("生成物体的总数：")] public int effectCount = 1; //生成的数量

    [Header("每个物体的间隔：")] public Vector3 interval = new Vector3(1, 1, 1); //每个特效的间隔
    [Header("开始生成的原点：")] public Vector3 initialPos = new Vector3(1, 1, 1); //生成特效的原点
    [Header("屏幕射线位置：")] public Vector2 ScreenRayPos = new Vector2(0.1f, 0.1f); //生成特效的原点

    [Header("每行数量：")] public Vector3 nn = new Vector3(1,1,1);

    private int _mm = 0;
    
    public float MenuPosX = 120.0f;
    public float MenuPosY = 120.0f;
    private bool MenuShow = false;

    // Start is called before the first frame update
    void Start()
    {
        //GenerateEff(effectCount, interval);
    }


    private void Update()
    {
        if (reset != false)
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                if (this.transform.childCount > 0)
                {
                    Destroy(this.transform.GetChild(i).gameObject);
                }
            }

            _mm = 0;
            GenerateEff(effectCount, interval);
            reset = false;
        }
    }

    private void OnGUI()
    {

        var rectBtnReset = new Rect(MenuPosX+224,MenuPosY+82,76,36);//输入框
        if (GUI.Button(rectBtnReset, "召唤菜单"))
        {
            var instanceEff = this.transform.GetComponent<EffectTestUI>();
            if (MenuShow == true)
            {
                instanceEff.enabled = false;
                MenuShow = false;
            }
            else
            {
                instanceEff.enabled = true;
                MenuShow = true;
            }
        }
    }

    public Vector3 GetPos()
    {
        //通过屏幕空间射线获得场景中的三维Pos
        var instanceEff2 = this.transform.GetComponent<EffectTestUI>();
        ScreenRayPos =  instanceEff2.ScreenRayPos;
        
        
        var ray= mainCam.ScreenPointToRay(new Vector2(ScreenRayPos.x * Screen.width, (1-ScreenRayPos.y) * Screen.height), Camera.MonoOrStereoscopicEye.Mono);
        float k = Mathf.Abs(ray.origin.y / ray.direction.y);
        var hit = ray.origin + ray.direction * k;
        Debug.Log("000000000000"+hit);
        Debug.DrawLine(mainCam.transform.position, hit, Color.yellow);
        
        return hit;
    }

    public void GenerateEff(int eC, Vector3 iE)
    {
        initialPos.x = GetPos().x;
        initialPos.z = GetPos().z;
        
        
        nn.x = effectCount / (nn.y * nn.z) + effectCount % (nn.y * nn.z);
        if (eC <= effectsPrefabs.Count)
        {
            for (int i = 0; i <= eC - 1; i++)
            {
                GameObject instances = Instantiate(effectsPrefabs[i],
                    new Vector3(initialPos.x + interval.x * i, initialPos.y, initialPos.z), transform.rotation);
            }
        }
        else
        {
            Vector3 wldPos = new Vector3(0, 0, 0);

            for (int i = 0; i < nn.x; i++)
            {
                wldPos.x = i * interval.x;
                for (int j = 0; j < nn.y; j++)
                {
                    wldPos.y = j * interval.y;
                    for (int k = 0; k < nn.z; k++)
                    {
                        wldPos.z = k * interval.z;
                        int kk = _mm % effectsPrefabs.Count;
                        if (_mm <= eC - 1)
                        {
                            GameObject instances = Instantiate(effectsPrefabs[kk],
                                wldPos + initialPos, transform.rotation);
                            instances.transform.SetParent(this.transform);
                            instances.name = "cube" + _mm;
                            _mm++;
                        }
                    }
                }
            }
        }
    }
}