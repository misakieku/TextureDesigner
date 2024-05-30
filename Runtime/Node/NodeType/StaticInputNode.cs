using System.Collections.Generic;
using UnityEngine;

namespace TextureDesigner
{
    public class StaticInputNode : TextureDesignerNode
    {
        public override bool AssignInput(Dictionary<int, Socket> input)
        {
            return true;
        }
    }
}
