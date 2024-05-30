using System;
using UnityEditor;

namespace TextureDesigner
{
    [Serializable]
    public class ExposedProperty
    {
        public string Name;
        public string Type;
        public string ID;
        public SerializedProperty Value; // This is just an example, you can use any type of value you want. You can use this as a base class and creating a new class that inherit this class to add more properties

        public ExposedProperty()
        {
            ID = Guid.NewGuid().ToString();
        }
    }
}
