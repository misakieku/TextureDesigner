using System;

namespace TextureDesigner.Editor
{
    public class CustomInspectorAttribute : Attribute
    {
        public Type inspectorType;

        public CustomInspectorAttribute(Type _inspectorType)
        {
            inspectorType = _inspectorType;
        }
    }
}