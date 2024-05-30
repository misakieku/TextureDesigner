using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TextureDesigner.Editor
{
    public class TextureDesignerEditorWindow : EditorWindow
    {
        private TextureDesignerAsset currentAsset;

        [SerializeField]
        private StyleSheet styleSheet;
        private SerializedObject serializedObject;
        private InspectorView inspectorView;
        private TextureDesignerGraphView graphView;
        private Toolbar toolbar;

        public static void Open(TextureDesignerAsset asset)
        {
            var window = Resources.FindObjectsOfTypeAll<TextureDesignerEditorWindow>().FirstOrDefault();

            if (window != null)
            {
                window.Clear();
                window.LoadAsset(asset);
                window.Focus();
                return;
            }

            window = CreateWindow<TextureDesignerEditorWindow>(typeof(TextureDesignerEditorWindow), typeof(SceneView));
            window.titleContent = new GUIContent(asset.name, EditorGUIUtility.GetIconForObject(asset));
            window.LoadAsset(asset);
        }

        private void Clear()
        {
            rootVisualElement.Clear();
            serializedObject = null;
            graphView = null;
            inspectorView = null;
        }

        private void OnGUI()
        {
            if (currentAsset != null)
            {
                hasUnsavedChanges = EditorUtility.IsDirty(currentAsset);
            }
        }

        private void OnEnable()
        {
            if (currentAsset != null)
            {
                DrawGraph();
            }
        }

        public void LoadAsset(TextureDesignerAsset asset)
        {
            currentAsset = asset;
            NodeLibrary.Instance.Load(currentAsset, true);

            DrawGraph();
        }

        private void DrawGraph()
        {
            serializedObject = new SerializedObject(currentAsset);

            var toolbar = new ToolbarView();

            // If resizable panel is needed for inspector and property panel, use TwoPaneSplitView or resizable element(BlackBoard or ResizableElement)
            var graphContainer = new VisualElement() { name = "GraphContainer" };
            graphContainer.style.flexDirection = FlexDirection.Row;
            graphContainer.style.flexGrow = 1;

            graphView = new TextureDesignerGraphView(serializedObject, styleSheet, this);
            inspectorView = new InspectorView(serializedObject);

            // Currently, the we don't need the property panel.
            // If needed, see PropertyBlackboardView.cs for implementation example
            //var propertiesPanel = new PropertyBlackboardView(graphView);

            // Must add the Blackboard to the graphView
            //graphView.Add(propertiesPanel);
            graphView.Add(inspectorView);

            // We can not directly add the graph view to the graphContainer since the RectangleSelector is calculate base on the parent position, so we need to add it to a container first
            var graphViewContainer = new VisualElement() { name = "GraphViewContainer" };
            graphViewContainer.style.flexGrow = 1;
            graphViewContainer.Add(graphView);

            graphContainer.Add(graphViewContainer);
            graphContainer.Add(inspectorView);

            var inspectorToggle = toolbar.Q<ToolbarToggle>("InspectorToggle");
            inspectorToggle.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                    inspectorView.style.display = DisplayStyle.Flex;
                else
                    inspectorView.style.display = DisplayStyle.None;
            });

            // Toggle to show/hide the property panel
            //var propertyToggle = toolbar.Q<ToolbarToggle>("PropertyToggle");
            //propertyToggle.RegisterValueChangedCallback(evt =>
            //{
            //    if (evt.newValue)
            //        propertiesPanel.style.display = DisplayStyle.Flex;
            //    else
            //        propertiesPanel.style.display = DisplayStyle.None;
            //});

            rootVisualElement.Add(toolbar);
            rootVisualElement.Add(graphContainer);
        }

        public void OnNodeSelect()
        {
            var selections = graphView.selection;
            inspectorView.OnNodeSelectionChanged(selections);
        }

        public void OnNodeUnselect(TextureDesignerEditorNode node)
        {
            // graphView.selection is not updated when a node is unselected, so we need to remove the node from the selection list
            var selections = new System.Collections.Generic.List<ISelectable>();
            selections.AddRange(graphView.selection);
            if (selections.Remove(node))
            {
                inspectorView.OnNodeSelectionChanged(selections);
            }
        }
    }
}
