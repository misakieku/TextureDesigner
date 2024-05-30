using System;

namespace TextureDesigner
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class NodeInputAttribute : Attribute
    {
        public string Name { get; }

        public NodeInputAttribute(string _name)
        {
            Name = _name;
        }

        public NodeInputAttribute()
        {
        }
    }
}
