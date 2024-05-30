using UnityEngine;

namespace TextureDesigner
{
    public class SingleChannelTextureSocket : TextureSocket
    {
        public SingleChannelTextureSocket(int _inputWidth, int _inputHeight, RenderTextureFormat format = RenderTextureFormat.R8) : base(_inputWidth, _inputHeight, format)
        {
        }
    }
}
