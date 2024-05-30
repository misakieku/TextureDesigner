using System.Collections.Generic;

namespace TextureDesigner
{
    /// <summary>
    /// Node library for the Texture Designer
    /// </summary>
    public class NodeLibrary // If you don't like to manage a singleton class that store all the node, you can let each socket referencing their owner (node) and linked socket
    {
        private static NodeLibrary _instance;
        public static NodeLibrary Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NodeLibrary();
                }
                return _instance;
            }
        }

        NodeLibrary()
        {
            nodes = new Dictionary<string, TextureDesignerNode>();
        }

        public TextureDesignerAsset CurrentAsset { get; private set; }

        private Dictionary<string, TextureDesignerNode> nodes;

        public void RegisterNode(TextureDesignerNode node)
        {
            if (!nodes.ContainsKey(node.ID))
            {
                nodes.Add(node.ID, node);
            }
        }

        public TextureDesignerNode GetNode(string id)
        {
            if (nodes.ContainsKey(id))
            {
                return nodes[id];
            }
            return null;
        }

        public void UnregisterNode(string id)
        {
            if (nodes.ContainsKey(id))
            {
                nodes.Remove(id);
            }
        }

        public void Clear()
        {
            nodes.Clear();
        }

        public void Load(TextureDesignerAsset asset, bool clear)
        {
            if (clear)
                Clear();

            CurrentAsset = asset;
            foreach (var node in asset.Nodes)
            {
                RegisterNode(node);
            }
        }
    }
}