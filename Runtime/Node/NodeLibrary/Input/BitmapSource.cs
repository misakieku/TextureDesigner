using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TextureDesigner
{
    [NodeInfo("Bitmap Source", "Input", false)]
    public class BitmapSource : TextureProcessingNode
    {
        [InspectorInput]
        public Texture2D Texture;

        [NodeOutput]
        public TextureSocket Output;
        [NodeOutput]
        public SingleChannelTextureSocket R;
        [NodeOutput]
        public SingleChannelTextureSocket G;
        [NodeOutput]
        public SingleChannelTextureSocket B;
        [NodeOutput]
        public SingleChannelTextureSocket A;

        public override bool AssignInput(Dictionary<int, Socket> input)
        {
            return true;
        }

        public override Socket[] Process()
        {
            Output = new TextureSocket(Texture.width, Texture.height);
            R = new SingleChannelTextureSocket(Texture.width, Texture.height);
            G = new SingleChannelTextureSocket(Texture.width, Texture.height);
            B = new SingleChannelTextureSocket(Texture.width, Texture.height);
            A = new SingleChannelTextureSocket(Texture.width, Texture.height);

            var computeShaderPath = ComputeShaderPath.GetPath("Input/BitmapSource");
            var computeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(computeShaderPath);

            var kernel = computeShader.FindKernel("CSMain");
            computeShader.SetTexture(kernel, "Input", Texture);
            computeShader.SetTexture(kernel, "Output", Output.Texture);
            computeShader.SetTexture(kernel, "R", R.Texture);
            computeShader.SetTexture(kernel, "G", G.Texture);
            computeShader.SetTexture(kernel, "B", B.Texture);
            computeShader.SetTexture(kernel, "A", A.Texture);
            computeShader.Dispatch(kernel, Texture.width / 8, Texture.height / 8, 1);

            return new Socket[] { Output, R, G, B, A };
        }
    }
}