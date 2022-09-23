using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace snippet_manager.Common
{
    public static class Extensions
    {
        public static EventHandlerList? DetachEvents(this Component? obj)
        {
            if (obj is null)
                return null;

            object? objNew = obj?.GetType()?.GetConstructor(Array.Empty<Type>())?.Invoke(Array.Empty<object>());
            PropertyInfo? propEvents = obj?.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);

            EventHandlerList? eventHandlerList_obj = (EventHandlerList?)propEvents?.GetValue(obj, null);
            EventHandlerList? eventHandlerList_objNew = (EventHandlerList?)propEvents?.GetValue(objNew, null);

            eventHandlerList_objNew?.AddHandlers(eventHandlerList_obj ?? new());
            eventHandlerList_obj?.Dispose();

            return eventHandlerList_objNew;
        }

        public static IEnumerable<T> SelectFrom<T>(this IDataReader reader,
                                      Func<IDataReader, T> projection)
        {
            while (reader.Read())
            {
                yield return projection(reader);
            }
        }

        public static IEnumerable<TreeNode> SearchTree(this TreeView treeView)
        {
            // return all nodes for treeview
            return treeView.Nodes.SearchTree();
        }
        public static IEnumerable<TreeNode> SearchTree(this TreeNodeCollection coll)
        {
            // return all nodes
            return coll.Cast<TreeNode>().Concat(coll.Cast<TreeNode>().SelectMany(x => x.Nodes.SearchTree()));
        }

    }
}