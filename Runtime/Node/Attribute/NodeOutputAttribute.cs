using System;

namespace TextureDesigner
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class NodeOutputAttribute : Attribute
    {
        public string Name { get; }

        public NodeOutputAttribute(string _name)
        {
            Name = _name;
        }

        public NodeOutputAttribute()
        {
        }
    }
}
