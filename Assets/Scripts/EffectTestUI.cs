using System.Collections;
using System.Collections.Generic;
using Tayx.Graphy.Utils.NumString;
using UnityEngine;
using UnityEngine.UI;
using Yoozoo.Core.Common;

public class EffectTestUI : MonoBehaviour
{
    public List<GameObject> effectsPrefabs;

    [Header("生成物体的总数：")] public int effectCount = 1; //生成的数量

    [Header("每个物体的间隔：")] public Vector3 interval = new Vector3(1, 1, 1); //每个特效的间隔
    [Header("开始生成的原点：")] public Vector3 initialPos = new Vector3(1, 1, 1); //生成特效的原点

    [Header("每行数量：")] public Vector3 nn = new Vector3(1,1,1);
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
        float x = 0.2f;
        float y = 0.2f;
        
        
        //数量
        GUI.Label(new Rect(24,50,48,24),"总数：");
        var rectInput = new Rect(72, 50, 50, 24);//输入框
        string reeffectCount = GUI.TextField(rectInput, effectCount.ToString());
        effectCount = reeffectCount.ToInt();

        
        //每行的数量x
        GUI.Label(new Rect(24,82,88,24),"每行数量xyz：");
        
        var rectCountx = new Rect(126, 82, 24, 24);//输入框
        string nnx = GUI.TextField(rectCountx, nn.x.ToString());
        nn.x = nnx.ToInt();
        //每行的数量y
        var rectCounty = new Rect(156, 82, 24, 24);//输入框
        string nny = GUI.TextField(rectCounty, nn.y.ToString());
        nn.y = nny.ToInt();
        //每行的数量z
        var rectCountz = new Rect(186, 82, 24, 24);//输入框
        string nnz = GUI.TextField(rectCountz, nn.z.ToString());
        nn.z = nnz.ToInt();
        
        
        //每个物体每个方向的间隔
        GUI.Label(new Rect(24,114,100,24),"每方向间隔xyz：");
        
        var rectJGx = new Rect(126, 114, 24, 24);//输入框
        string intervalx = GUI.TextField(rectJGx, interval.x.ToString());
        nn.x = nnx.ToInt();
        //每行的数量y
        var rectJGy = new Rect(156, 114, 24, 24);//输入框
        string intervaly = GUI.TextField(rectJGy, interval.y.ToString());
        nn.y = nny.ToInt();
        //每行的数量z
        var rectJGz = new Rect(186, 114, 24, 24);//输入框
        string intervalz = GUI.TextField(rectJGz, interval.z.ToString());
        nn.z = nnz.ToInt();

        var rectBtnReset = new Rect(200,126,66,66);//输入框
        if (GUI.Button(rectBtnReset, "Reset"))
        {
            var test = FindObjectOfType<EffectTest>();
            test.effectCount = count;
            test.GenerateEffects(count);
        }
        
    }
}
