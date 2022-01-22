using UnityEngine;
[ExecuteInEditMode]

public class EffectTestUI : MonoBehaviour
{
    //public List<GameObject> effectsPrefabs;
    [Header("生成物体的总数：")] public int effectCount = 20; //生成的数量

    public float effScale = 0.5f;
    
    [Header("选择品质类型：0-5/低端-高端")]
    public int EffQuality = 0;
    [Header("是否循环")]
    public bool effectLoop = true;
    [Header("循环间隔 秒")] public float loopNum = 3.0f;
    

    [Header("选择自动调整数量的轴：0-1/xz")]
    public int effAxis = 0;

    [Header("每个物体的间隔：")] public Vector3 interval = new Vector3(3, 3, 3); //每个特效的间隔
    [Header("开始生成的原点：")] public Vector3 initialPos = new Vector3(0, 0, 0); //生成特效的原点
    [Header("屏幕射线位置：")] public Vector2 ScreenRayPos = new Vector2(0.3f, 0.9f); //生成特效的原点

    [Header("每行数量(z轴会根据生成总数自动调整)：")] public Vector3 nn = new Vector3(1,1,10);

    public float MenuPosX = 24;
    public float MenuPosY = 24;

    private InstanceEffect newInsEff;
     
    //public float inputWidth = 0.1f;//屏幕尺寸百分比
    //public float inputHeigh = 0.1f;//屏幕尺寸百分比
    // Start is called before the first frame update

    private void Awake()
    {
        if (! this.GetComponent<InstanceEffect>())
        {
            InstanceEffect ds = gameObject.AddComponent(typeof(InstanceEffect)) as InstanceEffect;
            
        }
    }

    private void Start()
    {
        newInsEff = gameObject.GetComponent<InstanceEffect>();
    }

    private void Update()
    {
        newInsEff.effectCount = effectCount;
        newInsEff.interval = interval;
        newInsEff.nn = nn;
        newInsEff.effScale = effScale;
        newInsEff.effAxis = effAxis;
        newInsEff.EffQuality = EffQuality;
        newInsEff.effectLoop = effectLoop;
        newInsEff.loopNum = loopNum;
    }


    void OnGUI()
    {
        //在屏幕上标出射线开始发射的点
        GUI.Button(new Rect(ScreenRayPos.x * Screen.width, ScreenRayPos.y * Screen.height,Screen.width*0.025f,Screen.width*0.025f), "");
        
        //数量
        GUI.Label(new Rect(MenuPosX+24,MenuPosY+50,Screen.width*0.2f,24),"总数：");
        var rectInput = new Rect(MenuPosX+72, MenuPosY+50, 50, 24);//输入框
        string reeffectCount = GUI.TextField(rectInput, effectCount.ToString());
        int.TryParse(reeffectCount, out effectCount);

        //每行的数量x
        GUI.Label(new Rect(MenuPosX+24,MenuPosY+82,88,24),"每行数量xyz：");
        
        var rectCountx = new Rect(MenuPosX+126, MenuPosY+82, 24, 24);//输入框
        string nnx = GUI.TextField(rectCountx, nn.x.ToString());
        int.TryParse(nnx,out int nnxx);
        nn.x = nnxx;
        //每行的数量y
        var rectCounty = new Rect(MenuPosX+156, MenuPosY+82, 24, 24);//输入框
        string nny = GUI.TextField(rectCounty, nn.y.ToString());
        int.TryParse(nny, out int nnyy);
        nn.y = nnyy;
        //每行的数量z
        var rectCountz = new Rect(MenuPosX+186, MenuPosY+82, 24, 24);//输入框
        string nnz = GUI.TextField(rectCountz, nn.z.ToString());
        int.TryParse(nnz, out int nnzz);
        nn.z = nnzz;

        //每个物体每个方向的间隔
        GUI.Label(new Rect(MenuPosX+24,MenuPosY+114,100,24),"每方向间隔xyz：");

        var rectJGx = new Rect(MenuPosX+126, MenuPosY+114, 24, 24);//输入框
        string intervalx = GUI.TextField(rectJGx, interval.x.ToString());
        int.TryParse(intervalx, out int intervalxx);
        interval.x = intervalxx;
        //每行的数量y
        var rectJGy = new Rect(MenuPosX+156, MenuPosY+114, 24, 24);//输入框
        string intervaly = GUI.TextField(rectJGy, interval.y.ToString());
        int.TryParse(intervaly, out int intervalyy);
        interval.y = intervalyy;
        //每行的数量z
        var rectJGz = new Rect(MenuPosX+186,MenuPosY+114, 24, 24);//输入框
        string intervalz = GUI.TextField(rectJGz, interval.z.ToString());
        int.TryParse(intervalz, out int intervalzz);
        interval.z = intervalzz;

        //射线的屏幕空间位置
        GUI.Label(new Rect(MenuPosX+24,MenuPosY+146,100,24),"屏幕位置xy：");
        var rectRayx = new Rect(MenuPosX+126, MenuPosY+146, 32, 32);
        string ScreenRayPosx = GUI.TextField(rectRayx, ScreenRayPos.x.ToString());
        float.TryParse(ScreenRayPosx, out float ScreenRayPosxx);
        ScreenRayPos.x = ScreenRayPosxx;
        
        var rectRayy = new Rect(MenuPosX+166, MenuPosY+146, 32, 32);
        string ScreenRayPosy = GUI.TextField(rectRayy, ScreenRayPos.y.ToString());
        float.TryParse(ScreenRayPosy, out float ScreenRayPosyy);
        ScreenRayPos.y = ScreenRayPosyy;

        var rectBtnReset = new Rect(MenuPosX+224,MenuPosY+82,46,46);//输入框
        if (GUI.Button(rectBtnReset, "Reset"))
        {
            if (newInsEff.reset == true)
            {
                newInsEff.reset = false;
            }
            else
            {
                newInsEff.reset = true;
            }
        }

        //选择品质
        GUI.Label(new Rect(MenuPosX+290,MenuPosY+50,Screen.width*0.2f,24),"选择类型(0-5/低端-高端):");
        var rectQualityInput = new Rect(MenuPosX+450, MenuPosY+50, 32, 24);//输入框
        string rectQuality = GUI.TextField(rectQualityInput, EffQuality.ToString());
        int.TryParse(rectQuality, out int rectQualityy);
        EffQuality = rectQualityy;
        //循环
        GUI.Label(new Rect(MenuPosX+290,MenuPosY+82,Screen.width*0.2f,24),"开关循环：");
        var rectLoopInput = new Rect(MenuPosX+350, MenuPosY+82, 32, 24);//输入框
        bool rectLoop = GUI.Toggle(rectLoopInput, effectLoop,"");
        effectLoop = rectLoop;
        //循环次数
        GUI.Label(new Rect(MenuPosX+370,MenuPosY+82,Screen.width*0.2f,24),"循环间隔s：");
        var rectLoopNumInput = new Rect(MenuPosX+440, MenuPosY+82, 32, 24);//输入框
        string rectLoopNum = GUI.TextField(rectLoopNumInput, loopNum.ToString());
        int.TryParse(rectLoopNum, out int rectLoopNumm);
        loopNum = rectLoopNumm;
        //整体缩放
        GUI.Label(new Rect(MenuPosX+290,MenuPosY+114,Screen.width*0.2f,24),"整体缩放：");
        var rectScaleInput = new Rect(MenuPosX+350, MenuPosY+114, 32, 24);//输入框
        string rectScale = GUI.TextField(rectScaleInput, effScale.ToString());
        float.TryParse( rectScale, out float rectScalee);
        effScale = rectScalee;
        //自动调整数量的轴向
        GUI.Label(new Rect(MenuPosX+290,MenuPosY+146,Screen.width*0.2f,24),"自动调整数量的轴(0-1/x-z)：");
        var rectAxisInput = new Rect(MenuPosX+460, MenuPosY+146, 32, 24);//输入框
        string rectAxis = GUI.TextField(rectAxisInput, effAxis.ToString());
        int.TryParse(rectAxis, out int rectAxiss);
        effAxis = rectAxiss;
    }
}
