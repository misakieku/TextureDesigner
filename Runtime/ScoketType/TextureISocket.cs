using UnityEngine;

namespace TextureDesigner
{
    public class TextureSocket : Socket
    {
        public RenderTexture Texture => (RenderTexture)Value;

        public TextureSocket(int _inputWidth, int _inputHeight, RenderTextureFormat _textureFormat = RenderTextureFormat.ARGB32) : base(new RenderTexture(_inputWidth, _inputHeight, 0, _textureFormat), SocketValueType.Texture)
        {
            Texture.enableRandomWrite = true;
        }
    }
}