using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TextureDesigner.Editor
{
    internal class PropertyBlackboardView : Blackboard // You don't have to use Blackboard, but it's simple to implement. If you want to create your own view, you can use GraphElement and add a ResizableElement as child
    {
        internal PropertyBlackboardView(GraphView _graphView)
        {
            graphView = _graphView;

            Add(new BlackboardSection { title = "Exposed Properties" });

            addItemRequested = (blackboard) =>
            {
                // Creating a GenericMenu allows user to add a new property for selected type
                var menu = new GenericMenu();

                // You have to add and handle your own types here
                menu.AddItem(new GUIContent("New Property"), false, () =>
                {
                    var property = new ExposedProperty();
                    var container = new VisualElement();
                    var blackboardField = new BlackboardField { text = "New Property", typeText = property.Type };
                    container.Add(blackboardField);

                    var id = new Label(property.ID);
                    var blackboardRow = new BlackboardRow(blackboardField, id);

                    container.Add(blackboardRow);

                    Add(container);
                });

                menu.ShowAsContext();
            };
        }
    }
}
