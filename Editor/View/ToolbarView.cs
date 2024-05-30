using UnityEditor;
using UnityEngine.UIElements;

namespace TextureDesigner.Editor
{
    public class ToolbarView : VisualElement
    {
        private VisualElement toolbar;

        public ToolbarView()
        {
            var toolbarAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath.TOOLBAR);
            toolbar = toolbarAsset.Instantiate();

            Add(toolbar);
        }
    }
}
