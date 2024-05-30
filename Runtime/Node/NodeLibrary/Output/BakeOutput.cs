namespace TextureDesigner
{
    [NodeInfo("Bake Output", "Output", true, false)]
    public class BakeOutput : TextureProcessingNode
    {
        [InspectorInput]
        public string OutputPath;

        [NodeInput]
        public TextureSocket Input;
    }
}