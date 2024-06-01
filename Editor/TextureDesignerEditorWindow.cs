using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TextureDesigner.Editor
{
    public class TextureDesignerEditorWindow : EditorWindow
    {
        private TextureDesignerAsset currentAsset;

        private SerializedObject serializedObject;

        private InspectorView inspectorView;
        private TextureDesignerGraphView graphView;
        private ToolbarView toolbar;

        [MenuItem("Tools/TextureDesigner")]
        static void Open()
        {
            var window = CreateWindow<TextureDesignerEditorWindow>(typeof(TextureDesignerEditorWindow), typeof(SceneView));
            window.titleContent = new GUIContent("Texture Designer", EditorGUIUtility.IconContent(ConstAssets.EDITOR_WINDOW_ICON_PATH).image);
        }

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
            window.titleContent = new GUIContent(asset.name, EditorGUIUtility.IconContent(ConstAssets.EDITOR_WINDOW_ICON_PATH).image);

            window.LoadAsset(asset);
            window.DrawGraph();
            window.Initialize();
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
            DrawGraph();
            if (currentAsset != null)
            {
                LoadAsset(currentAsset);

                DrawGraph();
                Initialize();
            }
        }

        public void LoadAsset(TextureDesignerAsset asset)
        {
            Clear();

            currentAsset = asset;
            NodeLibrary.Instance.Load(currentAsset, true);
        }

        private void DrawGraph()
        {
            toolbar = new ToolbarView();
            toolbar.OnInspectorToggleTrigger = OnInspectorToggleTrigger;

            // If resizable panel is needed for inspector and property panel, use TwoPaneSplitView or resizable element(BlackBoard or ResizableElement)
            var graphContainer = new VisualElement() { name = "GraphContainer" };
            graphContainer.style.flexDirection = FlexDirection.Row;
            graphContainer.style.flexGrow = 1;

            graphView = new TextureDesignerGraphView(this);
            inspectorView = new InspectorView();

            // Register the OnNodeSelect and OnNodeUnselect event
            graphView.OnNodeSelect = OnNodeSelect;

            // Currently, the we don't need the property panel.
            // If needed, see PropertyBlackboardView.cs for implementation example
            //var propertiesPanel = new PropertyBlackboardView(graphView);

            // Must add the Blackboard to the graphView
            //graphView.Add(propertiesPanel);

            // We can not directly add the graph view to the graphContainer since the RectangleSelector is calculate base on the parent position, so we need to add it to a container first
            var graphViewContainer = new VisualElement() { name = "GraphViewContainer" };
            graphViewContainer.style.flexGrow = 1;
            graphViewContainer.Add(graphView);

            graphContainer.Add(graphViewContainer);
            graphContainer.Add(inspectorView);

            rootVisualElement.Add(toolbar);
            rootVisualElement.Add(graphContainer);

            // If no asset is loaded, show a warning label
            if (currentAsset == null)
            {
                RenderNoAssetAlert();
            }
        }

        private void RenderNoAssetAlert()
        {
            var warningLabelContainer = new VisualElement();
            var warningLabel = new Label("No asset loaded");

            // If you thing setting up the style in code is too much, you can create a uxml file and instantiate it here
            warningLabel.style.fontSize = 20;
            warningLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

            warningLabelContainer.style.flexGrow = 1;
            warningLabelContainer.style.justifyContent = Justify.Center;
            warningLabelContainer.style.alignItems = Align.Center;
            warningLabelContainer.style.backgroundColor = new Color(0f, 0f, 0f, 0.5f);
            // Set the position to absolute to make sure the warning label is on top of the graph view
            warningLabelContainer.style.position = Position.Absolute;
            warningLabelContainer.style.width = new Length(100, LengthUnit.Percent);
            warningLabelContainer.style.height = new Length(100, LengthUnit.Percent);

            warningLabelContainer.Add(warningLabel);

            rootVisualElement.Add(warningLabelContainer);
        }

        private void Initialize()
        {
            serializedObject = new SerializedObject(currentAsset);
            toolbar.Initialize(serializedObject);
            graphView.InitializeGraph(serializedObject);
            inspectorView.Initialize(serializedObject);

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            graphView.ResetGraph();
            graphView.InitializeGraph(serializedObject);
        }

        private void OnNodeSelect()
        {
            var selections = graphView.selection;
            inspectorView.OnNodeSelectionChanged(selections);
        }

        private void OnInspectorToggleTrigger(bool isON)
        {
            if (isON)
                inspectorView.style.display = DisplayStyle.Flex;
            else
                inspectorView.style.display = DisplayStyle.None;
        }
    }
}
