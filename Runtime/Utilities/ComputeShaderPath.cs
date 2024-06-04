using UnityEngine;

namespace TextureDesigner
{
    public static class ComputeShaderPath
    {
        public static string GetPath(string name)
        {
            return "Packages/com.misaki.texturedesigner/Runtime/Node/ComputeShader/" + name + ".compute";
        }
    }
}
