using UnityEngine;

namespace TextureDesigner
{
    public class ColorSocket : TextureSocket
    {
        public ColorSocket(Color _color) : base(1, 1, RenderTextureFormat.ARGB32)
        {
            RenderTexture.active = Texture;
            GL.Begin(GL.TRIANGLES);
            GL.Clear(true, true, _color);
            GL.End();
            RenderTexture.active = null;
        }
    }
}