using NodeGraphControl.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraphEditor.Nodes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class NodeTypeAttribute : Attribute
    {
        public string NodeType { get; }

        public string ContextName { get; set; }

        public string ContextCategory { get; set; }

        public Type JsonParamsType { get; set; }

        public string ParamsTypeName { get; set; }

        public NodeTypeAttribute(string type)
        {
            NodeType = type;
            ContextName = type;
            ContextCategory = "Other";
            ParamsTypeName = type;
        }

        public static List<Type> GetAttributedTypes()
        {
            var typesWithAttribute = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsDefined(typeof(NodeTypeAttribute), inherit: true))
                .ToList();

            return typesWithAttribute;
        }

        public static bool TryGetAttribute(Type type, out NodeTypeAttribute nodeTypeAttribute)
        {
            nodeTypeAttribute = (NodeTypeAttribute)GetCustomAttribute(type, typeof(NodeTypeAttribute));
            return nodeTypeAttribute != null;
        }
    }
}
