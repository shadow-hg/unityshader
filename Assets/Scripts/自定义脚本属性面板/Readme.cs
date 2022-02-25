using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yoozoo.Arts.Scene
{
    public class Readme : MonoBehaviour
    {
        //这里写逻辑
        public GUIStyle labStyle;

        public List<string> Tips = new List<string>()
        {
            "1、层级：将角色拖动到<Role>层级下,并选中角色，在属性面板的右上角将Layer修改为“commander1”",
            "2、位置：如果角色太靠近摄像机，可以将<Role>往后移动，注意请将角色XYZ轴保持为0",
            "3、材质：为角色设置好对应的角色专用材质。",
            "4、匹配材质：选中<Role>后在右侧属性面板点击“一键寻找”。",
            "5、查看效果：运行场景后在Game窗口查看效果，鼠标左右滑动旋转角色。",
            "6、动画：如果角色带有动画，运行时会自动播放。",
            "（本场景渲染效果与游戏中的“头目”界面中角色的材质效果是一样的。）"
        };
    }
}
