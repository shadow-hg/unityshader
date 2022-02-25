using UnityEditor;

namespace Yoozoo.Arts.Scene.Editor
{
    [CanEditMultipleObjects, CustomEditor(typeof(Readme))]
    public class ReadmeEditor : UnityEditor.Editor
    {
        //这里写UI
        private Readme _readme;

        void OnEnable()
        {
            _readme = (target as Readme);
        }

        public void AllStyles()
        {
            // labStyle = new GUIStyle();
            // labStyle.fontSize = 14;
            // labStyle.normal.textColor = Color.white;
        }

        public override void OnInspectorGUI()
        {
            //AllStyles();

            foreach (var tip in _readme.Tips)
            {
                EditorGUILayout.LabelField(tip);
            }
        }
    }
}