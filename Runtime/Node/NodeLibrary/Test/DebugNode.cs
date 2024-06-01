using System.Collections.Generic;
using UnityEngine;

namespace TextureDesigner
{
    [NodeInfo("Debug", "Test")]
    public class DebugNode : TextureDesignerNode
    {
        [NodeInput]
        public StringSocket Input;

        [NodeOutput]
        public StringSocket Output;

        public override bool AssignInput(Dictionary<int, Socket> input)
        {
            if (input.TryGetValue(0, out var socket))
            {
                Input = socket as StringSocket;
                return true;
            }

            return false;
        }

        public override Socket[] Process()
        {
            Debug.Log(Input.Text);
            Output = new StringSocket(Input.Text);
            return new Socket[] { Output };
        }
    }
}
