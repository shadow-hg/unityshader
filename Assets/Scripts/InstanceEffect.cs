using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class InstanceEffect : MonoBehaviour
{
    public bool reset = false;
    [Space(25)]
    public List<GameObject> effectsPrefabs;

    [Header("生成物体的总数：")] public int effectCount = 1; //生成的数量

    [Header("每个物体的间隔：")] public Vector3 interval = new Vector3(1, 1, 1); //每个特效的间隔
    [Header("开始生成的原点：")] public Vector3 initialPos = new Vector3(1, 1, 1); //生成特效的原点

    [Header("每行数量：")] public Vector3 nn = new Vector3(1,1,1);

    private int _mm = 0;

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
                Destroy(this.transform.GetChild(i).gameObject);
            }

            _mm = 0;
            GenerateEff(effectCount, interval);
            reset = false;
        }
    }


    public void GenerateEff(int eC, Vector3 iE)
    {
        nn.z = effectCount / (nn.y * nn.x) + effectCount % (nn.y * nn.x);
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

            for (int i = 0; i < nn.y; i++)
            {
                wldPos.y = i * interval.y;
                for (int j = 0; j < nn.x; j++)
                {
                    wldPos.x = j * interval.x;
                    for (int k = 0; k < nn.z; k++)
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