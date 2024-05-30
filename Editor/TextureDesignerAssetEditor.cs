using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace TextureDesigner.Editor
{
    [CustomEditor(typeof(TextureDesignerAsset))]
    public class TextureDesignerAssetEditor : UnityEditor.Editor
    {
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID) as TextureDesignerAsset;

            if (asset != null)
            {
                TextureDesignerEditorWindow.Open(asset);
                return true;
            }

            return false;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor"))
            {
                TextureDesignerEditorWindow.Open((TextureDesignerAsset)target);
            }

            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
