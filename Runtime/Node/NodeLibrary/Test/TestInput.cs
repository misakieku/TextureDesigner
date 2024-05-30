using UnityEngine;

namespace TextureDesigner
{
    [NodeInfo("Test Input", "Test", false, false)]
    public class TestInput : TextureDesignerNode
    {
        [InspectorInput]
        public float TestFloatValue;
        [InspectorInput]
        public int TestIntValue;
        [InspectorInput]
        public string TestStringValue;
        [InspectorInput]
        public Vector2 TestVector2Value;
        [InspectorInput]
        public Vector3 TestVector3Value;
        [InspectorInput]
        public Vector4 TestVector4Value;
        [InspectorInput]
        public Color TestColorValue;
        [InspectorInput]
        public Matrix4x4 TestMatrix4x4Value;
    }
}
