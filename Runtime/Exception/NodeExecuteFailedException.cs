namespace TextureDesigner
{
    internal class NodeExecuteFailedException : System.Exception
    {
        public string NodeID { get; }

        public NodeExecuteFailedException(string _nodeID, string _message) : base(_message)
        {
            NodeID = _nodeID;
        }
    }
}
