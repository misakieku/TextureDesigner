using UnityEngine;

namespace TextureDesigner
{
    public class StringSocket : Socket
    {
        public string Text => (string)Value;

        public StringSocket() : base(string.Empty, SocketValueType.String)
        {
        }

        public StringSocket(string _value) : base(_value,  SocketValueType.String)
        {
        }
    }
}
