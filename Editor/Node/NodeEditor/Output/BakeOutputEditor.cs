using UnityEditor;
using UnityEngine.UIElements;

namespace TextureDesigner.Editor
{
    [CustomInspector(typeof(BakeOutput))]
    public class BakeOutputEditor : TextureDesignerEditorNode
    {
        BakeOutput target;
        public BakeOutputEditor(TextureDesignerNode _node) : base(_node)
        {
            target = _node as BakeOutput;
        }

        public override VisualElement OnInspector(SerializedObject serializedObject)
        {
            var root = base.OnInspector(serializedObject);

            var button = new Button(() =>
            {
                var path = EditorUtility.SaveFolderPanel("Select output path", "", "");
                if (!string.IsNullOrEmpty(path))
                {
                    // Since ui toolkit use binding to update the value on UI, we just need to update the value of the property
                    target.OutputPath = path;
                }
            });
            button.text = "Select output path";

            root.Add(button);

            return root;
        }
    }
}
