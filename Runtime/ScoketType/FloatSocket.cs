namespace TextureDesigner
{
    public class FloatSocket : Socket
    {
        public float FloatValue => (float)Value;
        public FloatSocket(float value) : base(value, SocketValueType.Float)
        {
        }
    }
}
