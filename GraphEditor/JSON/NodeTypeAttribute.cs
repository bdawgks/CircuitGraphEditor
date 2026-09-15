using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.JSON
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class NodeTypeAttribute : Attribute
    {
        public string NodeType { get; }

        public Type JsonParamsType { get; set; }

        public NodeTypeAttribute(string type)
        {
            NodeType = type;
        }
    }
}
