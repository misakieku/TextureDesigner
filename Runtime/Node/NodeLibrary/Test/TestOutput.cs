using System;
using System.Collections.Generic;
using UnityEngine;

namespace TextureDesigner
{
    [NodeInfo("Test Output", "Output", true, false)]
    public class TestOutput : TextureDesignerNode
    {
        [NodeInput]
        public Socket Input;

        public override bool AssignInput(Dictionary<int, Socket> input)
        {
            if (input.TryGetValue(0, out var socket))
            {
                Input = socket;
                return true;
            }

            return false;
        }

        public override Socket[] Process()
        {
            Debug.Log($"Test Output: Get input from {InputConnections[0].OutputPort.NodeID}, socket type {Input.ValueType}");

            return Array.Empty<Socket>();
        }
    }
}