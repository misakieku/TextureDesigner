using System.Collections.Generic;
using UnityEngine;

namespace TextureDesigner
{
    public enum BlendMode
    {
        Burn, 
        Darken, 
        Difference, 
        Dodge, 
        Divide, 
        Exclusion, 
        HardLight, 
        HardMix, 
        Lighten, 
        LinearBurn, 
        LinearDodge, 
        LinearLight, 
        LinearLightAddSub, 
        Multiply, 
        Negation, 
        Overlay, 
        PinLight, 
        Screen, 
        SoftLight, 
        Subtract, 
        VividLight, 
        Overwrite
    }

    [NodeInfo("Blend", "Atomic Nodes")]
    public class Blend : TextureProcessingNode
    {
        [InspectorInput]
        public BlendMode BlendMode;
        [Range(0, 1)]
        [InspectorInput]
        public float Opacity;

        [NodeInput]
        public ColorSocket Foreground;
        [NodeInput]
        public ColorSocket Background;
        [NodeInput]
        public SingleChannelTextureSocket Mask;

        [NodeOutput]
        public ColorSocket Output;

        public override bool AssignInput(Dictionary<int, Socket> input)
        {
            if (input.ContainsKey(0) && input.ContainsKey(1) && input.ContainsKey(2))
            {
                Foreground = input[0] as ColorSocket;
                Background = input[1] as ColorSocket;
                Mask = input[2] as SingleChannelTextureSocket;

                InputWidth = Foreground.Texture.width;
                InputHeight = Foreground.Texture.height;

                return true;
            }

            return false;
        }
    }
}