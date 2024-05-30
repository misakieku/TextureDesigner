using System.Collections.Generic;
using UnityEngine;

namespace TextureDesigner
{
    public class RenderTexturePool
    {
        private Stack<RenderTexture> pool;
        private int width;
        private int height;

        public RenderTexturePool(int _initialSize, int _width, int _height)
        {
            pool = new Stack<RenderTexture>(_initialSize);
            width = _width;
            height = _height;

            for (var i = 0; i < _initialSize; i++)
            {
                var rt = new RenderTexture(_width, _height, 0);
                pool.Push(rt);
            }
        }

        public RenderTexture Get()
        {
            if (pool.Count > 0)
            {
                return pool.Pop();
            }
            else
            {
                var rt = new RenderTexture(width, height, 0);
                pool.Push(rt);
                return rt;
            }
        }

        public void Return(RenderTexture rt)
        {
            pool.Push(rt);
        }
    }
}
