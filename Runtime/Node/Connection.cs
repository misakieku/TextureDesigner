using System;

namespace TextureDesigner
{
    /// <summary>
    /// Represents a connection between two connection ports.
    /// </summary>
    [Serializable]
    public struct Connection
    {
        public ConnectionPort InputPort;

        public ConnectionPort OutputPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> struct.
        /// </summary>
        /// <param name="_inputPort">The input connection port.</param>
        /// <param name="_outputPort">The output connection port.</param>
        public Connection(ConnectionPort _inputPort, ConnectionPort _outputPort)
        {
            InputPort = _inputPort;
            OutputPort = _outputPort;
        }
    }

    /// <summary>
    /// Represents a connection port.
    /// </summary>
    [Serializable]
    public struct ConnectionPort
    {
        public string NodeID;
        public int PortIndex;

        public ConnectionPort(string _nodeID, int _portIndex)
        {
            NodeID = _nodeID;
            PortIndex = _portIndex;
        }
    }
}
