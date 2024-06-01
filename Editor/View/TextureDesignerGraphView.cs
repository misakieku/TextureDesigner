using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace TextureDesigner.Editor
{
    public class TextureDesignerGraphView : GraphView
    {
        private SerializedObject serializedObject;
        private TextureDesignerAsset currentAsset;
        public TextureDesignerAsset CurrentAsset => currentAsset;

        private Dictionary<string, TextureDesignerEditorNode> editorNodeLibrary;
        private Dictionary<Edge, Connection> connectionLibrary;

        public Action OnNodeSelect;

        private GridBackground gridBackground;
        private NodeSearchProvider searchProvider;

        private TextureDesignerEditorWindow window;
        public TextureDesignerEditorWindow Window => window;
        public TextureDesignerGraphView(TextureDesignerEditorWindow _windows)
        {
            window = _windows;

            style.flexGrow = 1;
            styleSheets.Add(ConstAssets.StyleSheet);

            gridBackground = new GridBackground() { name = "grid" };
            Add(gridBackground);
            gridBackground.SendToBack();
        }

        public void InitializeGraph(SerializedObject _serializedObject)
        {
            serializedObject = _serializedObject;
            currentAsset = (TextureDesignerAsset)serializedObject.targetObject;
            editorNodeLibrary = new Dictionary<string, TextureDesignerEditorNode>();
            connectionLibrary = new Dictionary<Edge, Connection>();

            graphViewChanged -= OnGraphViewChanged;

            searchProvider = ScriptableObject.CreateInstance<NodeSearchProvider>();
            searchProvider.GraphView = this;

            nodeCreationRequest = context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchProvider);
            };

            viewTransform.position = currentAsset.GraphPosition;
            viewTransform.scale = currentAsset.GraphScale;

            // Those are the unity built-in manipulators for graph view
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
            this.AddManipulator(new ContentZoomer());

            InitializeAssetElements();

            graphViewChanged += OnGraphViewChanged;
        }

        private void InitializeAssetElements()
        {
            if (currentAsset.Nodes.Count > 0)
            {
                foreach (var node in currentAsset.Nodes)
                {
                    AddEditorNodeToView(node);
                }
            }

            if (currentAsset.Connections.Count > 0)
            {
                foreach (var connection in currentAsset.Connections)
                {
                    AddConnectionToView(connection);
                }
            }
        }

        public override void RemoveFromSelection(ISelectable selectable)
        {
            base.RemoveFromSelection(selectable);
            OnNodeSelect?.Invoke();
        }

        public void ResetGraph()
        {
            // Need to unsubscribe the event to prevent the graph from being reset multiple times
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            editorNodeLibrary.Clear();
            connectionLibrary.Clear();
            graphViewChanged += OnGraphViewChanged;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                var removedElements = graphViewChange.elementsToRemove;
                Undo.RecordObject(currentAsset, $"Remove elements");

                for (var i = removedElements.Count - 1; i >= 0; i--)
                {
                    if (removedElements[i] is TextureDesignerEditorNode node)
                    {
                        var asset = (TextureDesignerAsset)serializedObject.targetObject;
                        asset.Nodes.Remove(node.Node);
                        editorNodeLibrary.Remove(node.Node.ID);
                        NodeLibrary.Instance.UnregisterNode(node.Node.ID);
                    }

                    if (removedElements[i] is Edge edge)
                    {
                        if (connectionLibrary.TryGetValue(edge, out var connection))
                        {
                            currentAsset.Connections.Remove(connection);
                            connectionLibrary.Remove(edge);

                            if (editorNodeLibrary.TryGetValue(connection.InputPort.NodeID, out var inputEditorNode))
                            {
                                inputEditorNode.Node.InputConnections.Remove(connection);
                            }
                        }
                    }
                }
            }

            if (graphViewChange.movedElements != null)
            {
                var movedElements = graphViewChange.movedElements;
                Undo.RecordObject(currentAsset, $"Move {movedElements.FirstOrDefault().GetType().Name}");

                foreach (var element in graphViewChange.movedElements)
                {
                    if (element is TextureDesignerEditorNode node)
                    {
                        node.SavePosition(node.GetPosition());
                    }
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                var edges = graphViewChange.edgesToCreate;
                Undo.RecordObject(currentAsset, $"Connect {edges.FirstOrDefault().GetType().Name}");

                foreach (var edge in edges)
                {
                    if (edge.input.node is TextureDesignerEditorNode inputEditorNode && edge.output.node is TextureDesignerEditorNode outputEditorNode)
                    {
                        var inputIndex = inputEditorNode.Ports.IndexOf(edge.input);
                        var outputIndex = outputEditorNode.Ports.IndexOf(edge.output);

                        var connection = new Connection(new ConnectionPort(inputEditorNode.Node.ID, inputIndex), new ConnectionPort(outputEditorNode.Node.ID, outputIndex));
                        currentAsset.Connections.Add(connection);
                        connectionLibrary.Add(edge, connection);

                        inputEditorNode.Node.InputConnections.Add(connection);
                    }
                }
            }

            currentAsset.SaveTransform(viewTransform);

            serializedObject.Update();
            EditorUtility.SetDirty(currentAsset);
            return graphViewChange;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort != port
                    && startPort.node != port.node
                    && startPort.direction != port.direction
                    && startPort.portType.IsAssignableFrom(port.portType) || port.portType.IsAssignableFrom(startPort.portType))
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        private void RemoveNode(TextureDesignerNode node)
        {
            currentAsset.Nodes.Remove(node);

            serializedObject.Update();
        }

        public void AddNode(TextureDesignerNode node)
        {
            Undo.RecordObject(currentAsset, $"Add {node.GetType().Name}");

            currentAsset.Nodes.Add(node);
            NodeLibrary.Instance.RegisterNode(node);

            EditorUtility.SetDirty(currentAsset);
            serializedObject.Update();

            AddEditorNodeToView(node);
        }

        private void AddEditorNodeToView(TextureDesignerNode node)
        {
            node.TypeName = node.GetType().Name;
            TextureDesignerEditorNode editorNode;

            // Replace the inspector editor to a custom inspector if it has one
            var types = TypeCache.GetTypesWithAttribute<CustomInspectorAttribute>();
            var type = types.FirstOrDefault(t => t.GetCustomAttribute<CustomInspectorAttribute>().inspectorType == node.GetType());
            if (type != null)
                editorNode = (TextureDesignerEditorNode)System.Activator.CreateInstance(type, node);
            else
                editorNode = new TextureDesignerEditorNode(node);

            editorNode.SetPosition(node.Position);
            editorNode.OnNodeSelect = OnNodeSelect;

            editorNodeLibrary.Add(node.ID, editorNode);

            AddElement(editorNode);
        }

        private void AddConnectionToView(Connection connection)
        {
            if (editorNodeLibrary.TryGetValue(connection.InputPort.NodeID, out var inputNode)
                && editorNodeLibrary.TryGetValue(connection.OutputPort.NodeID, out var outputNode))
            {
                var inputPort = inputNode.Ports[connection.InputPort.PortIndex];
                var outputPort = outputNode.Ports[connection.OutputPort.PortIndex];

                var edge = inputPort.ConnectTo(outputPort);

                connectionLibrary.Add(edge, connection);

                AddElement(edge);
            }
        }

    }
}