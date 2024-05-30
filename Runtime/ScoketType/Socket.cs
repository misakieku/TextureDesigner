namespace TextureDesigner
{
    public enum SocketValueType
    {
        Float,
        String,
        Color,
        Texture,
        SingleChannelTexture,
    }

    /// <summary>
    /// Represents a socket that holds a value of a specific type.
    /// </summary>
    public class Socket // We use Socket instead of the actual value type such as float, string, etc. is because this allows us to create a generic system that can be used for any type of value.
    {
        /// <summary>
        /// Gets the value of the socket.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets the type of value stored in the socket.
        /// </summary>
        public SocketValueType ValueType { get; } // Currently, this is not very useful.

        /// <summary>
        /// Initializes a new instance of the Socket class with the specified value and value type.
        /// </summary>
        /// <param name="_value">The value to be stored in the socket.</param>
        /// <param name="_valueType">The type of value to be stored in the socket.</param>
        public Socket(object _value, SocketValueType _valueType)
        {
            Value = _value;
            ValueType = _valueType;
        }
    }
}
