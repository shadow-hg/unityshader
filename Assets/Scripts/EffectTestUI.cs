using System.Collections;
using System.Collections.Generic;
using Tayx.Graphy.Utils.NumString;
using UnityEngine;
using UnityEngine.UI;
using Yoozoo.Core.Common;

public class EffectTestUI : MonoBehaviour
{
    //public List<GameObject> effectsPrefabs;

    [Header("生成物体的总数：")] public int effectCount = 1; //生成的数量

    [Header("每个物体的间隔：")] public Vector3 interval = new Vector3(1, 1, 1); //每个特效的间隔
    [Header("开始生成的原点：")] public Vector3 initialPos = new Vector3(1, 1, 1); //生成特效的原点
    [Header("屏幕射线位置：")] public Vector2 ScreenRayPos = new Vector2(0.1f, 0.1f); //生成特效的原点

    [Header("每行数量：")] public Vector3 nn = new Vector3(1,1,1);

    public float MenuPosX = 24;
    public float MenuPosY = 24;

    
    public float inputWidth = 0.1f;//屏幕尺寸百分比
    public float inputHeigh = 0.1f;//屏幕尺寸百分比
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    public void GenerateEffects(int count)
    {
        
    }

    private string countText = "10";

    private int count = 10;

    /*void OnGUI()
    {
        var rect = new Rect(50, 50, 200, 50);

        string newText = GUI.TextField(rect, countText);
        if (newText != countText)
        {
            if (int.TryParse(countText, out count))
            {
                countText = newText;
            }
        }

        rect.y += 60;

        if (GUI.Button(rect, "DO"))
        {
            var test = FindObjectOfType<EffectTest>();
            test.effectCount = count;
            test.GenerateEffects(count);
        }
    }*/

    void OnGUI()
    {
        //在屏幕上标出射线开始发射的点
        GUI.Button(new Rect(ScreenRayPos.x * Screen.width, ScreenRayPos.y * Screen.height,5,5), "");
        
        //数量
        GUI.Label(new Rect(MenuPosX+24,MenuPosY+50,Screen.width*0.2f,24),"总数：");
        var rectInput = new Rect(MenuPosX+72, MenuPosY+50, 50, 24);//输入框
        string reeffectCount = GUI.TextField(rectInput, effectCount.ToString());
        effectCount = reeffectCount.ToInt();

        
        //每行的数量x
        GUI.Label(new Rect(MenuPosX+24,MenuPosY+82,88,24),"每行数量xyz：");
        
        var rectCountx = new Rect(MenuPosX+126, MenuPosY+82, 24, 24);//输入框
        string nnx = GUI.TextField(rectCountx, nn.x.ToString());
        nn.x = nnx.ToInt();
        //每行的数量y
        var rectCounty = new Rect(MenuPosX+156, MenuPosY+82, 24, 24);//输入框
        string nny = GUI.TextField(rectCounty, nn.y.ToString());
        nn.y = nny.ToInt();
        //每行的数量z
        var rectCountz = new Rect(MenuPosX+186, MenuPosY+82, 24, 24);//输入框
        string nnz = GUI.TextField(rectCountz, nn.z.ToString());
        nn.z = nnz.ToInt();
        
        
        //每个物体每个方向的间隔
        GUI.Label(new Rect(MenuPosX+24,MenuPosY+114,100,24),"每方向间隔xyz：");
        
        var rectJGx = new Rect(MenuPosX+126, MenuPosY+114, 24, 24);//输入框
        string intervalx = GUI.TextField(rectJGx, interval.x.ToString());
        interval.x = intervalx.ToInt();
        //每行的数量y
        var rectJGy = new Rect(MenuPosX+156, MenuPosY+114, 24, 24);//输入框
        string intervaly = GUI.TextField(rectJGy, interval.y.ToString());
        interval.y = intervaly.ToInt();
        //每行的数量z
        var rectJGz = new Rect(MenuPosX+186,MenuPosY+114, 24, 24);//输入框
        string intervalz = GUI.TextField(rectJGz, interval.z.ToString());
        interval.z = intervalz.ToInt();
        
        //射线的屏幕空间位置
        GUI.Label(new Rect(MenuPosX+24,MenuPosY+146,100,24),"屏幕位置xy：");
        var rectRayx = new Rect(MenuPosX+126, MenuPosY+146, 32, 32);
        string ScreenRayPosx = GUI.TextField(rectRayx, ScreenRayPos.x.ToString());
        ScreenRayPos.x = ScreenRayPosx.ToFloat();
        
        var rectRayy = new Rect(MenuPosX+166, MenuPosY+146, 32, 32);
        string ScreenRayPosy = GUI.TextField(rectRayy, ScreenRayPos.y.ToString());
        ScreenRayPos.y = ScreenRayPosy.ToFloat();

        var rectBtnReset = new Rect(MenuPosX+224,MenuPosY+82,46,46);//输入框
        if (GUI.Button(rectBtnReset, "Reset"))
        {
            var instanceEff = this.transform.GetComponent<InstanceEffect>();
            instanceEff.effectCount = effectCount;
            instanceEff.interval = interval;
            instanceEff.nn = nn;
            instanceEff.reset = true;
            interval.x = instanceEff.interval.x;
        }
        
    }
}
