using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TextureDesigner.Editor
{
    public class TextureDesignerGraphView : GraphView
    {
        private SerializedObject serializedObject;
        private TextureDesignerAsset currentAsset;
        private TextureDesignerEditorWindow window;
        public TextureDesignerEditorWindow Window => window;
        public TextureDesignerAsset CurrentAsset => currentAsset;

        private Dictionary<string, TextureDesignerEditorNode> EditorNodeLibrary = new();
        private Dictionary<Edge, Connection> ConnectionLibrary = new();


        private NodeSearchProvider searchProvider;
        public TextureDesignerGraphView(SerializedObject _serializedObject, StyleSheet _styleSheet, TextureDesignerEditorWindow _window)
        {
            serializedObject = _serializedObject;
            currentAsset = (TextureDesignerAsset)serializedObject.targetObject;
            window = _window;

            searchProvider = ScriptableObject.CreateInstance<NodeSearchProvider>();
            searchProvider.GraphView = this;

            nodeCreationRequest = context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchProvider);
            };

            var gridBackground = new GridBackground() { name = "grid" };
            Add(gridBackground);
            gridBackground.SendToBack();
            style.flexGrow = 1;
            styleSheets.Add(_styleSheet);

            // Those are the unity built-in manipulators for graph view
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
            this.AddManipulator(new ContentZoomer());

            InitializeGraph();

            graphViewChanged += OnGraphViewChanged;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                var removedElements = graphViewChange.elementsToRemove;
                Undo.RecordObject(serializedObject.targetObject, $"Remove {removedElements.FirstOrDefault().GetType().Name}");

                for (var i = removedElements.Count - 1; i >= 0; i--)
                {
                    if (removedElements[i] is TextureDesignerEditorNode node)
                    {
                        var asset = (TextureDesignerAsset)serializedObject.targetObject;
                        asset.Nodes.Remove(node.Node);
                        EditorNodeLibrary.Remove(node.Node.ID);
                        NodeLibrary.Instance.UnregisterNode(node.Node.ID);
                    }

                    if (removedElements[i] is Edge edge)
                    {
                        if (ConnectionLibrary.TryGetValue(edge, out var connection))
                        {
                            currentAsset.Connections.Remove(connection);
                            ConnectionLibrary.Remove(edge);

                            if (EditorNodeLibrary.TryGetValue(connection.InputPort.NodeID, out var inputEditorNode))
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
                        ConnectionLibrary.Add(edge, connection);

                        inputEditorNode.Node.InputConnections.Add(connection);
                    }
                }
            }

            serializedObject.Update();
            EditorUtility.SetDirty(currentAsset);
            return graphViewChange;
        }

        private void InitializeGraph()
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
            editorNode.OnNodeSelect = window.OnNodeSelect;
            editorNode.OnNodeUnselect = window.OnNodeUnselect;

            EditorNodeLibrary.Add(node.ID, editorNode);

            AddElement(editorNode);
        }

        private void AddConnectionToView(Connection connection)
        {
            if (EditorNodeLibrary.TryGetValue(connection.InputPort.NodeID, out var inputNode)
                && EditorNodeLibrary.TryGetValue(connection.OutputPort.NodeID, out var outputNode))
            {
                var inputPort = inputNode.Ports[connection.InputPort.PortIndex];
                var outputPort = outputNode.Ports[connection.OutputPort.PortIndex];

                var edge = inputPort.ConnectTo(outputPort);

                ConnectionLibrary.Add(edge, connection);

                AddElement(edge);
            }
        }
    }
}