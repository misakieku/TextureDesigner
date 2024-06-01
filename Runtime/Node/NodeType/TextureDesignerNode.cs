using System;
using System.Collections.Generic;
using UnityEngine;

namespace TextureDesigner
{
    [Serializable]
    public class TextureDesignerNode
    {
        [SerializeField]
        private string guid;
        [SerializeField]
        private Rect position;

        public string TypeName;

        public string ID => guid;
        public Rect Position => position;

        private List<Connection> inputConnections;
        public List<Connection> InputConnections => inputConnections;

        public int InputPortCount;
        public int OutputPortCount;

        public TextureDesignerNode()
        {
            guid = Guid.NewGuid().ToString();
            inputConnections = new List<Connection>();
        }

        /// <summary>
        /// Sets the position of the node.
        /// </summary>
        /// <param name="position">The position to set.</param>
        public void SetPosition(Rect position)
        {
            this.position = position;
        }

        /// <summary>
        /// Retrieves the input from the connected nodes and processes it.
        /// </summary>
        /// <returns>A dictionary containing the results obtained from connected nodes with input port index.</returns>
        public virtual Dictionary<int, Socket> GetInput()
        {
            var inputResults = new Dictionary<int, Socket>();

            foreach (var connection in inputConnections)
            {
                var outputNode = NodeLibrary.Instance.GetNode(connection.OutputPort.NodeID);
                if (outputNode != null)
                {
                    var needExecution = outputNode.AssignInput(outputNode.GetInput());

                    if (needExecution)
                    {
                        var output = outputNode.Process();
                        var outputIndex = connection.OutputPort.PortIndex - outputNode.InputPortCount;
                        inputResults.Add(outputIndex, output[outputIndex]);
                    }
                    else
                    {
                        throw new NodeExecuteFailedException(outputNode.ID, $"AssignInput returned false, which means the node {outputNode.ID} is not handled properly.");
                    }
                }
            }

            return inputResults;
        }

        /// <summary>
        /// Assigns the input dictionary to the node. In most cases, this method should not be overridden.
        /// </summary>
        /// <param name="input">The dictionary containing the input sockets with input port index.</param>
        /// <returns>Returns true if assigning is done, otherwise returns false. Please note that returning false will throw an exception when executing the node.</returns>
        public virtual bool AssignInput(Dictionary<int, Socket> input)
        {
            return false;
        }

        /// <summary>
        /// Processes the input and returns the resulting sockets.
        /// </summary>
        /// <returns>An array of sockets sorted with the output port.</returns>
        public virtual Socket[] Process()
        {
            // Use Array.Empty<Socket>() to return an empty array without allocating memory.
            return Array.Empty<Socket>();
        }

        /// <summary>
        /// Executes the node by retrieving the input, assigning it, and processing it.
        /// </summary>
        public void Execute()
        {
            var input = GetInput();
            AssignInput(input);
            Process();
        }
    }
}
