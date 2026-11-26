using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeGraphControl.Elements
{
    public class Group
    {
        readonly int _id;
        readonly HashSet<AbstractNode> _nodes = new HashSet<AbstractNode>();
        
        public int Id { get => _id; }

        public HashSet<AbstractNode> Nodes { get => _nodes; }

        public Group(int id)
        {
            _id = id;
        }
    }
}
