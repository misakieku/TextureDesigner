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
    }
}