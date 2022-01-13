using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class InstanceEffect : MonoBehaviour
{
    
    public Camera mainCam;
    [Header("选择品质：0-2/低端-高端")]
    public int EffQuality = 0;
    
    [Header("播放：")]
    public bool reset = false;
    [Header("是否循环")]
    public bool effectLoop = true;

    [Header("循环间隔 秒")] public float loopNum = 3.0f;
    
    [Space(25)][Header("各品质下需要播放的特效")]
    public List<GameObject> effectsPrefabs_Low;
    public List<GameObject> effectsPrefabs_Middle;
    public List<GameObject> effectsPrefabs_Heigh;
    public List<GameObject> effectsPrefabs_PTgonji;
    public List<GameObject> effectsPrefabs_HuDun;
    public List<GameObject> effectsPrefabs_Shouji;
    
    [Header("*生成物体的总数(带*号的参数在EffectTestUI上优先级更大)：")]
    public int effectCount = 1; //生成的数量
    [Header("*选择自动调整数量的轴：0-1/xz")]
    public int effAxis = 0;
    [Header("*整体缩放：")]
    public float effScale = 0.5f;
    [Header("*每个物体的间隔：")] public Vector3 interval = new Vector3(1, 1, 1); //每个特效的间隔
    [Header("*开始生成的原点：")] public Vector3 initialPos = new Vector3(1, 1, 1); //生成特效的原点
    [Header("*屏幕射线位置：")] public Vector2 ScreenRayPos = new Vector2(0.1f, 0.9f); //生成特效的原点

    [Header("*每行数量：")] public Vector3 nn = new Vector3(1,1,1);

    private int _mm = 0;
    
    public float MenuPosX = 120.0f;
    public float MenuPosY = 120.0f;
    private bool MenuShow = false;

    float gameTime = 0;

    private List<GameObject> effectsPrefabs;
    
    // Start is called before the first frame update
    void Start()
    {
        //GenerateEff(effectCount, interval);
    }


    private void Update()
    {
        if (reset == true )
        {
            if (effectLoop == true)
            {
                //延迟每loopNum秒后播放一次
                InvokeRepeating("EffPlay",0,loopNum);
                reset = false;
            }else
            {
                EffPlay();
                reset = false;
            }
        }
        if (effectLoop == false)
        {
            CancelInvoke("EffPlay");
        }

    }

    public void EffPlay()
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
    }
    
    private void OnGUI()
    {

        var rectBtnReset = new Rect(MenuPosX+266,MenuPosY+100,76,36);//输入框
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
        this.transform.position = GetPos();

        this.transform.localScale = new Vector3(1, 1, 1);

        //this.transform.rotation = new Vector3(0,mainCam.transform.rotation.y,0);

        //切换品质
        switch (EffQuality)
        {
            case 0:
                effectsPrefabs = effectsPrefabs_Low;
                break;
            case 1:
                effectsPrefabs = effectsPrefabs_Middle;
                break;
            case 2:
                effectsPrefabs = effectsPrefabs_Heigh;
                break;
            case 3:
                effectsPrefabs = effectsPrefabs_PTgonji;
                break;
            case 4:
                effectsPrefabs = effectsPrefabs_HuDun;
                break;
            case 5:
                effectsPrefabs = effectsPrefabs_Shouji;
                break;
}
        
        //切换自动控制数量的轴向
        switch (effAxis)
        {
            case 0:
                nn.x = effectCount / (nn.y * nn.z) + effectCount % (nn.y * nn.z);
                break;
            case 1:
                nn.z = effectCount / (nn.y * nn.x) + effectCount % (nn.y * nn.x);
                break;
        }

        if (eC < effectsPrefabs.Count)
        {
            for (int i = 0; i <= eC - 1; i++)
            {
                GameObject instances = Instantiate(effectsPrefabs[i],
                    new Vector3(initialPos.x + interval.x * i, initialPos.y, initialPos.z), transform.rotation);
                instances.transform.SetParent(this.transform);
                instances.name = "cube" + _mm;
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
        this.transform.localScale = new Vector3(effScale, effScale, effScale);
    }
}