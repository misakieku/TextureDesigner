namespace TextureDesigner
{
    [NodeInfo("Float", "Input", false)]
    public class FloatInput : StaticInputNode
    {
        [InspectorInput]
        public float Value;

        [NodeOutput]
        public Socket Output;

        public override Socket[] Process()
        {
            Output = new Socket(Value, SocketValueType.Float);

            return new Socket[] { Output };
        }
    }
}