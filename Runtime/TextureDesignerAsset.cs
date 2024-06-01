using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TextureDesigner
{
    [CreateAssetMenu(fileName = "Texture Designer Asset", menuName = "Editor Tools/Texture Designer Asset")]
    public class TextureDesignerAsset : ScriptableObject
    {
        // We must use SerializeReference to serialize the list of nodes
        [SerializeReference]
        private List<TextureDesignerNode> nodes;
        public List<TextureDesignerNode> Nodes => nodes;

        [SerializeField]
        private List<Connection> connections;
        public List<Connection> Connections => connections;

        //[SerializeField]
        //private List<ExposedProperty> exposedProperties;
        //public List<ExposedProperty> ExposedProperties => exposedProperties;

        public int GlobalTextureWidth = 2048;
        public int GlobalTextureHeight = 2048;

        public Vector3 GraphPosition;
        public Vector3 GraphScale;

        public TextureDesignerAsset()
        {
            nodes = new List<TextureDesignerNode>();
            connections = new List<Connection>();
            //exposedProperties = new List<ExposedProperty>();

            GraphPosition = Vector3.zero;
            GraphScale = Vector3.one;
        }

        public void SaveTransform(ITransform transform)
        {
            GraphPosition = transform.position;
            GraphScale = transform.scale;
        }

        public void SaveAsset()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public void SaveAssetAs(string path)
        {
            AssetDatabase.CreateAsset(this, path);
            SaveAsset();
        }

        public void Compute()
        {
            foreach (var node in nodes)
            {
                // We only want to compute the output nodes since they are the ones that will generate the final result.
                // The GetInput method will recursively get the input from the connected nodes in include sub-connection.
                // The Process method will process the input and return the result.
                if (node is BakeOutput outputNode)
                {
                    //node.Execute();
                }

                if (node is TestOutput)
                {
                    node.Execute();
                }
            }
        }

    }
}
