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

        internal InspectorView(SerializedObject _serializedObject)
        {
            style.width = 300;

            serializedObject = _serializedObject;

            var uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath.INSPECTOR);
            propertiesContainerAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath.INSPECTOR_PROPERTY_CONTAINER);

            var root = uiAsset.Instantiate();
            root.style.flexGrow = 1;
            root.pickingMode = PickingMode.Ignore;
            inspectorRoot = root.Q<ScrollView>("InspectorRoot");

            Add(root);
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
