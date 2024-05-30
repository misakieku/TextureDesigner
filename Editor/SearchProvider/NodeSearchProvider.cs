using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TextureDesigner.Editor
{
    public struct SearchContextElement
    {
        public object Target { get; private set; }
        public string Title { get; private set; }

        public SearchContextElement(object _target, string _title)
        {
            Target = _target;
            Title = _title;
        }
    }

    public class NodeSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        public TextureDesignerGraphView GraphView;
        public VisualElement NodeCreationView;
        public static List<SearchContextElement> SearchContextElements;

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>();
            // The first entry is the main group, which will be shown as title of the search window
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Node"), 0));

            SearchContextElements = new List<SearchContextElement>();

            // Use type cache to get all types derived from TextureDesignerNode instead of using reflection, this can improve the search speed significantly
            var types = TypeCache.GetTypesDerivedFrom<TextureDesignerNode>();

            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;

                if (type.CustomAttributes.Count() > 0)
                {
                    var nodeInfo = type.GetCustomAttribute<NodeInfoAttribute>();
                    if (!string.IsNullOrEmpty(nodeInfo.Category))
                    {
                        string title = $"{nodeInfo.Category}/{nodeInfo.Name}";
                        var node = Activator.CreateInstance(type);
                        SearchContextElements.Add(new SearchContextElement(node, title));
                    }
                }
            }

            // Sort by name
            SearchContextElements.Sort((a, b) =>
            {
                var splits1 = a.Title.Split('/');
                var splits2 = b.Title.Split('/');
                for (int i = 0; i < splits1.Length; i++)
                {
                    if (splits2.Length <= i)
                        return 1;

                    var compare = string.Compare(splits1[i], splits2[i]);
                    if (compare != 0)
                    {
                        if (splits1.Length != splits2.Length && (i == splits1.Length - 1 || i == splits2.Length - 1))
                            return splits1.Length > splits2.Length ? 1 : -1;

                        return compare;
                    }
                }

                return 0;
            });

            // Build the tree
            foreach (var element in SearchContextElements)
            {
                var entryTitle = element.Title.Split('/');
                var lastGroup = tree.FindLast(e => e is SearchTreeGroupEntry && e.name == entryTitle[0]);
                if (lastGroup == null)
                {
                    lastGroup = new SearchTreeGroupEntry(new GUIContent(entryTitle[0]), 1);
                    tree.Add(lastGroup);
                }

                var groupName = string.Empty;
                for (int i = 0; i < entryTitle.Length - 1; i++)
                {
                    groupName += entryTitle[i];
                    var group = tree.FindLast(e => e is SearchTreeGroupEntry && e.name == groupName);
                    if (group == null)
                    {
                        group = new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1);
                        tree.Add(group);
                    }

                    groupName += "/";
                }

                var entry = new SearchTreeEntry(new GUIContent(entryTitle[entryTitle.Length - 1]))
                {
                    level = entryTitle.Length,
                    userData = new SearchContextElement(element.Target, element.Title)
                };

                tree.Add(entry);
            }

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var windowMousePosition = GraphView.ChangeCoordinatesTo(GraphView.contentContainer, context.screenMousePosition - GraphView.Window.position.position);
            var graphMousePosition = GraphView.contentViewContainer.WorldToLocal(windowMousePosition);

            var element = (SearchContextElement)SearchTreeEntry.userData;

            var node = element.Target as TextureDesignerNode;
            node.SetPosition(new Rect(graphMousePosition, Vector2.zero));
            GraphView.AddNode(node);

            return true;
        }
    }
}