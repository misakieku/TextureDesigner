using System;

namespace TextureDesigner
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class NodeInfoAttribute : Attribute
    {
        public string Name { get; }
        public string Category { get; }
        public bool HasInput { get; }
        public bool HasOutput { get; }

        public NodeInfoAttribute(string name, string category, bool hasInput = true, bool hasOutput = true)
        {
            Name = name;
            Category = category;
            HasInput = hasInput;
            HasOutput = hasOutput;
        }
    }
}