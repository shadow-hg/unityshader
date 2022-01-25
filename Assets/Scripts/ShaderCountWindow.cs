using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
 
public class ShaderCountWindow : EditorWindow
{
    public static string assetFolderPath = "Assets/HRP/HRPshader/EffectShader";
    public static string dataSavePath;
    [MenuItem ("Tools/资源合规/材质/统计变体数量")]
    public static void ShowWindow () {
        EditorWindow thisWindow = EditorWindow.GetWindow(typeof(ShaderCountWindow));
        thisWindow.titleContent = new GUIContent("统计shader变体数据");
        thisWindow.position = new Rect(Screen.width/2, Screen.height/2, 600, 1000);
    }
 
    public static void GetAllShaderVariantCount()
    {
        Assembly asm = Assembly.GetAssembly(typeof(UnityEditor.SceneView));
        // Assembly asm = Assembly.LoadFile(@"D:\Unity\Unity2018.4.7f1\Editor\Data\Managed\UnityEditor.dll");
        System.Type t2 = asm.GetType("UnityEditor.ShaderUtil");
        MethodInfo method = t2.GetMethod("GetVariantCount", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Debug.Log(Application.dataPath);
        string projectPath = Application.dataPath.Replace("Assets", "");
        Debug.Log(projectPath);
        assetFolderPath = assetFolderPath.Replace(projectPath, "");
        var shaderList = AssetDatabase.FindAssets("t:Shader", new[] {assetFolderPath});
 
        // var output = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
        string date = System.DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
        string pathF = string.Format("{0}/ShaderVariantCount{1}.txt", dataSavePath, date);
        FileStream fs = new FileStream(pathF, FileMode.Create, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
 
        EditorUtility.DisplayProgressBar("Shader统计文件", "正在写入统计文件中...", 0f);
        int ix = 0;
        sw.WriteLine("Shader 数量：" + shaderList.Length);
        sw.WriteLine("ShaderFile, VariantCount");
        int totalCount = 0;
        foreach(var i in shaderList)
        {
            EditorUtility.DisplayProgressBar("Shader统计文件", "正在写入统计文件中...", ix/shaderList.Length);
            var path = AssetDatabase.GUIDToAssetPath(i);
            Shader s = AssetDatabase.LoadAssetAtPath(path, typeof(Shader)) as Shader;
            var variantCount = method.Invoke(null, new System.Object[] { s, true});
            if (int.Parse(variantCount.ToString()) > 20){
                sw.WriteLine(path + "," + variantCount.ToString() + "!!!!!!!!!!!!!!!!!!!");
            }else{
                sw.WriteLine(path + "," + variantCount.ToString());
            }
            totalCount += int.Parse(variantCount.ToString());
            ++ix;
        }
        sw.WriteLine("Shader Variant Total Amount: " + totalCount);
        EditorUtility.ClearProgressBar();
        sw.Close();
        fs.Close();
        OpenFolder();
    }
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("选择文件夹");
        EditorGUILayout.TextField(assetFolderPath);
        if (GUILayout.Button("选择"))
        {
            assetFolderPath = EditorUtility.OpenFolderPanel("选择文件夹", assetFolderPath, "");
        }
        EditorGUILayout.EndHorizontal();
 
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("选择存储位置");
        EditorGUILayout.TextField(dataSavePath);
        if (GUILayout.Button("选择"))
        {
            dataSavePath = EditorUtility.OpenFolderPanel("选择存储位置", dataSavePath, "");
        }
        EditorGUILayout.EndHorizontal();
        
 
        if (GUILayout.Button("开始计算") && assetFolderPath != null && dataSavePath != null)
        {
            GetAllShaderVariantCount();
        }
    }
 
    public static void OpenFolder(){
        string path = dataSavePath.Replace("/", "\\");
        System.Diagnostics.Process.Start("explorer.exe", path);
    }
}