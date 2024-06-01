namespace TextureDesigner
{
    [NodeInfo("String", "Input", false)]
    public class StringInput : StaticInputNode
    {
        [NodeOutput]
        public StringSocket Out;

        [InspectorInput]
        public string Value;

        public override Socket[] Process()
        {
            Out = new StringSocket(Value);
            return new Socket[] { Out };
        }
    }
}
