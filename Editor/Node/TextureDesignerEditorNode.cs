using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TextureDesigner.Editor
{
    public class TextureDesignerEditorNode : Node
    {
        private TextureDesignerNode node;
        public TextureDesignerNode Node => node;

        private Type nodeType;

        private List<Port> ports;
        public List<Port> Ports => ports;

        public Action OnNodeSelect;
        public Action<TextureDesignerEditorNode> OnNodeUnselect;

        public TextureDesignerEditorNode(TextureDesignerNode _node)
        {
            // Add to the class list so that we can style the node using uss selectors
            AddToClassList("texture-designer-node");

            node = _node;
            ports = new List<Port>();

            nodeType = node.GetType();
            var nodeInfo = nodeType.GetCustomAttribute<NodeInfoAttribute>();

            title = nodeInfo.Name;

            // Add the category as a class to the node so that we can style the node based on the category
            var depths = nodeInfo.Category.Split('/').ToList();
            depths.Add(nodeInfo.Name);
            foreach (var depth in depths)
            {
                AddToClassList(depth.ToLower().Replace(" ", "-"));
            }

            name = nodeInfo.Name;

            if (nodeInfo.HasInput)
            {
                var fields = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(field => field.GetCustomAttributes(typeof(NodeInputAttribute)).Any());

                foreach (var field in fields)
                {
                    CreateInputPort(field);
                }
                node.InputPortCount = fields.Count();
            }
            else
            {
                inputContainer.style.display = DisplayStyle.None;
            }

            if (nodeInfo.HasOutput)
            {
                var fields = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(field => field.GetCustomAttributes(typeof(NodeOutputAttribute)).Any());

                foreach (var field in fields)
                {
                    CreateOutputPort(field);
                }
                node.OutputPortCount = fields.Count();
            }
            else
            {
                outputContainer.style.display = DisplayStyle.None;
            }
        }

        private void CreateInputPort(FieldInfo field)
        {
            var inputAttribute = field.GetCustomAttribute<NodeInputAttribute>();
            var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, field.FieldType);

            inputPort.portName = string.IsNullOrEmpty(inputAttribute.Name) ? ObjectNames.NicifyVariableName(field.Name) : inputAttribute.Name;
            inputPort.userData = field.GetValue(node);
            inputPort.portColor = PortColor.GetColor(field.FieldType);

            inputContainer.Add(inputPort);
            ports.Add(inputPort);
        }

        private void CreateOutputPort(FieldInfo field)
        {
            var outputAttribute = field.GetCustomAttribute<NodeOutputAttribute>();
            var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, field.FieldType);

            outputPort.portName = string.IsNullOrEmpty(outputAttribute.Name) ? ObjectNames.NicifyVariableName(field.Name) : outputAttribute.Name;
            outputPort.userData = field.GetValue(node);
            outputPort.portColor = PortColor.GetColor(field.FieldType);

            outputContainer.Add(outputPort);
            ports.Add(outputPort);
        }

        public void SavePosition(Rect _position)
        {
            node.SetPosition(_position);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelect?.Invoke();
        }

        public override void OnUnselected()
        {
            OnNodeUnselect?.Invoke(this);
        }

        /// <summary>
        /// Displays the inspector for the node.
        /// </summary>
        /// <param name="serializedObject">The serialized object containing the node data.</param>
        /// <returns>The root visual element of the inspector.</returns>
        public virtual VisualElement OnInspector(SerializedObject serializedObject)
        {
            var root = new VisualElement();

            // Use reflection to get the inspector input fields
            var fields = nodeType.GetFields().Where(f => f.GetCustomAttribute<InspectorInputAttribute>() != null);

            if (fields.Count() == 0)
            {
                var label = new Label("No properties to display.");
                root.Add(label);

                return root;
            }

            foreach (var field in fields)
            {
                // Find all the nodes in the serialized object
                var property = serializedObject.FindProperty("nodes");
                if (!property.isArray)
                    continue;

                var arraySize = property.arraySize;
                for (var i = 0; i < arraySize; i++)
                {
                    var element = property.GetArrayElementAtIndex(i);
                    var nodeID = element.FindPropertyRelative("guid").stringValue;

                    if (nodeID != node.ID)
                        continue;

                    var customName = field.GetCustomAttribute<InspectorInputAttribute>().Name;
                    var serializedProperty = element.FindPropertyRelative(field.Name);

                    // Use PropertyField to create a field for the property, this will automatically handle the correct field type
                    var inputField = new PropertyField(serializedProperty);
                    // Use ObjectNames.NicifyVariableName to convert the field name to a more readable format
                    inputField.label = string.IsNullOrEmpty(customName) ? ObjectNames.NicifyVariableName(field.Name) : customName;
                    inputField.name = field.Name;
                    inputField.Bind(serializedObject);
                    //InstallManipulator(inputField, serializedProperty);

                    root.Add(inputField);
                }
            }

            return root;
        }

        // Todo: Implement a custom manipulator to add a context menu to the property field that allows users to expose the property to graph.
        void InstallManipulator(VisualElement element, SerializedProperty property)
        {
            var m = new ContextualMenuManipulator((evt) =>
            {
                evt.menu.AppendAction("Test", action =>
                {
                    //var exposedProperty = new ExposedProperty() { Name = property.displayName, Type = property.type, Value = property };
                    //NodeLibrary.Instance.CurrentAsset.ExposedProperties.Add(exposedProperty);
                }, DropdownMenuAction.AlwaysEnabled);
            });

            m.target = element;
        }
    }
}