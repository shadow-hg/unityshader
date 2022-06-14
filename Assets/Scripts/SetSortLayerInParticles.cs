using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace Yoozoo.Arts.EffectTool
{
    [ExecuteInEditMode]
    public class SetSortLayerInParticles : Checker
    {
    
        private string dir = "Assets/ResourcesAssets/Prefabs/Effect/Battle/Rts/";
        //private static SetSortLayerInParticles _instance;
        void Start()
        {
            GetAllParticles();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        [MenuItem("Tools/特效工具/特效层级设置")]
        private static void Init()
        {
            SetSortLayerInParticles instance = new SetSortLayerInParticles();
            instance.GetAllParticles();
        }

        private void GetAllParticles()
        {
            DirectoryInfo direction = new DirectoryInfo(dir);
            FileInfo[] sfxPrefabs = direction.GetFiles("*", SearchOption.TopDirectoryOnly);
            for(int i = 0; i < sfxPrefabs.Length; i++)
            {
                if(sfxPrefabs[i].Name.EndsWith(".prefab"))//取脚本文件
                {
                    
                    Debug.Log(sfxPrefabs[i].Directory.GetFiles());
                    
                }
            }
        }
    }
}

