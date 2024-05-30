using System;

namespace TextureDesigner
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class InspectorInputAttribute : Attribute
    {
        public string Name { get; }

        public InspectorInputAttribute(string _name)
        {
            Name = _name;
        }

        public InspectorInputAttribute()
        {
        }
    }
}
