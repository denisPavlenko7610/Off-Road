using UnityEditor;
using UnityEngine;
namespace Off_Road.Editor
{
    public class ChangeShader : EditorWindow
    {
        [MenuItem("Tools/Change Shader to Standard")]
        public static void ShowWindow()
        {
            string[] guids = AssetDatabase.FindAssets("t:Material", null);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat.shader.name.Contains("Universal Render Pipeline"))
                {
                    mat.shader = Shader.Find("Standard");
                }
            }
        }
    }
}