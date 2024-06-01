using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.UIElements;

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

        public override VisualElement CreateInspectorGUI()
        {
            var asset = (TextureDesignerAsset)target;

            var root = new VisualElement();

            var openButton = new Button(() =>
            {
                TextureDesignerEditorWindow.Open(asset);
            })
            {
                text = "Open Editor"
            };

            var saveButton = new Button(() =>
            {
                asset.SaveAsset();
            })
            {
                text = "Save"
            };

            var computeButton = new Button(() =>
            {
                asset.Compute();
            })
            {
                text = "Compute"
            };

            root.Add(openButton);
            root.Add(saveButton);
            root.Add(computeButton);

            return root;
        }
    }
}
