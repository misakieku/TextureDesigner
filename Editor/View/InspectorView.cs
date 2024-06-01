using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace TextureDesigner.Editor
{
    internal class InspectorView : GraphElement
    {
        private SerializedObject serializedObject;
        private VisualElement inspectorRoot;

        private VisualTreeAsset propertiesContainerAsset;

        internal InspectorView()
        {
            style.width = 300;

            var uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ConstAssets.INSPECTOR);
            propertiesContainerAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ConstAssets.INSPECTOR_PROPERTY_CONTAINER);

            var root = uiAsset.Instantiate();
            root.style.flexGrow = 1;
            inspectorRoot = root.Q<ScrollView>("InspectorRoot");

            Add(root);
        }

        public void Initialize(SerializedObject _serializedObject)
        {
            serializedObject = _serializedObject;
        }

        internal void OnNodeSelectionChanged(List<ISelectable> selections)
        {
            inspectorRoot.Clear();

            foreach (var selection in selections)
            {
                if (selection is TextureDesignerEditorNode editorNode)
                {
                    var propertiesContainer = propertiesContainerAsset.Instantiate();
                    var nodeHeader = propertiesContainer.Q<Label>("Header");
                    var propertiesRoot = propertiesContainer.Q<VisualElement>("PropertiesContainer");

                    nodeHeader.text = editorNode.Node.TypeName;
                    propertiesRoot.Add(editorNode.OnInspector(serializedObject));

                    inspectorRoot.Add(propertiesContainer);
                }
            }
        }

    }
}
