using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class InstanceEffect : MonoBehaviour
{
    public List<GameObject> effectsPrefabs;

    [Header("生成物体的总数：")] public int effectCount = 1; //生成的数量

    [Header("每个物体的间隔：")] public Vector3 interval = new Vector3(1, 1, 1); //每个特效的间隔
    [Header("开始生成的原点：")] public Vector3 initialPos = new Vector3(1, 1, 1); //生成特效的原点

    [Header("每行数量：")] public int nn = 0;

    private int _mm = 0;

    // Start is called before the first frame update
    void Start()
    {
        GenerateEff(effectCount, interval);
    }


    /*void ThreeRoot(int a)
    {
        while (_mm*_mm*_mm <= a)
        {
            _nn = Mathf.Abs(_mm * _mm * _mm - a); 
            _mm++;
        }
    }*/


    public void GenerateEff(int eC, Vector3 iE)
    {
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

            for (int i = 0; i < nn; i++)
            {
                wldPos.y = i * interval.y;
                for (int j = 0; j < nn; j++)
                {
                    wldPos.x = j * interval.x;
                    for (int k = 0; k < nn; k++)
                    {
                        wldPos.z = k * interval.z;
                        int kk = _mm % effectsPrefabs.Count;
                        if (_mm <= eC - 1)
                        {
                            GameObject instances = Instantiate(effectsPrefabs[kk],
                                wldPos, transform.rotation);
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