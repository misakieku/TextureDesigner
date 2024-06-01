using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TextureDesigner.Editor
{
    public static class ConstAssets
    {
        public const string EDITOR_WINDOW_ICON_PATH = "Packages/com.misaki.texturedesigner/Icon/TextureDesignerEditor.png";

        private const string UXML_FOLDER_PATH = "Packages/com.misaki.texturedesigner/Editor/View/UXML/";

        public const string INSPECTOR_PROPERTY_CONTAINER = UXML_FOLDER_PATH + "InspectorPropertiesContainer.uxml";
        public const string INSPECTOR = UXML_FOLDER_PATH + "InspectorView.uxml";
        public const string TOOLBAR = UXML_FOLDER_PATH + "ToolBar.uxml";
        public const string PROPERTY_PANEL = UXML_FOLDER_PATH + "PropertyPanelView.uxml";

        public const string STYLE_SHEET_PATH = "Packages/com.misaki.texturedesigner/Editor/Style/Style.uss";
        private static StyleSheet styleSheet;
        public static StyleSheet StyleSheet
        {

            get
            {
                if (styleSheet == null)
                {
                    styleSheet = LoadStyleSheet();
                }
                return styleSheet;
            }
        }

        private static StyleSheet LoadStyleSheet()
        {
            return AssetDatabase.LoadAssetAtPath<StyleSheet>(STYLE_SHEET_PATH);
        }
    }
}
