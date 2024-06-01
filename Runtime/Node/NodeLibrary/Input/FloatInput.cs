namespace TextureDesigner
{
    [NodeInfo("Float", "Input", false)]
    public class FloatInput : StaticInputNode
    {
        [InspectorInput]
        public float Value;

        [NodeOutput]
        public FloatSocket Output;

        public override Socket[] Process()
        {
            Output = new FloatSocket(Value);

            return new Socket[] { Output };
        }
    }
}