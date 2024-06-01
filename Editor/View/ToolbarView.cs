using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TextureDesigner.Editor
{
    public class ToolbarView : VisualElement
    {
        private SerializedObject serializedObject;

        public Action<bool> OnInspectorToggleTrigger;

        public ToolbarView()
        {
            var toolbarAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ConstAssets.TOOLBAR);
            var toolbar = toolbarAsset.Instantiate();

            Add(toolbar);
        }

        public void Initialize(SerializedObject _serializedObject)
        {
            serializedObject = _serializedObject;

            var currentAsset = (TextureDesignerAsset)serializedObject.targetObject;

            var saveButton = this.Q<Button>("SaveButton");
            saveButton.clicked += () =>
            {
                currentAsset.SaveAsset();
            };

            var saveAsButton = this.Q<Button>("SaveAsButton");
            saveAsButton.clicked += () =>
            {
                var path = EditorUtility.SaveFilePanelInProject("Save Texture Designer Asset", "Texture Designer Asset", "asset", "Please enter a file name to save the asset to");
                if (!string.IsNullOrEmpty(path))
                {
                    currentAsset.SaveAssetAs(path);
                }
            };

            var computeButton = this.Q<Button>("ComputeButton");
            computeButton.clicked += () =>
            {
                currentAsset.Compute();
            };

            var inspectorToggle = this.Q<ToolbarToggle>("InspectorToggle");
            inspectorToggle.RegisterValueChangedCallback(evt =>
            {
                OnInspectorToggleTrigger?.Invoke(evt.newValue);
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
        }
    }
}
