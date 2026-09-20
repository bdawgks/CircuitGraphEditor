using NodeGraphControl.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeGraphControl
{
    public class GraphSelection
    {
        readonly List<AbstractNode> _nodes = new List<AbstractNode>();
        readonly List<Wire> _connections = new List<Wire>();

        public List<AbstractNode> Nodes { get => _nodes; }
        public List<Wire> Connections { get => _connections; }
        public Point Origin { get; set; } = new Point(0, 0);

        public GraphSelection() { }

        public GraphSelection(NodeGraphControl graph, bool selectedOnly = false)
        {
            if (selectedOnly)
            {
                foreach (AbstractNode node in graph.Nodes.Where(node => node.Selected))
                {
                    _nodes.Add(node);
                }
                foreach (Wire wire in graph.Connections.
                    Where(wire => wire.From.Parent.Selected && wire.To.Parent.Selected))
                {
                    _connections.Add(wire);
                }
            }
            else
            {
                foreach (AbstractNode node in graph.Nodes)
                {
                    _nodes.Add(node);
                }
                foreach (Wire wire in graph.Connections)
                {
                    _connections.Add(wire);
                }
            }
        }

        public void AddToGraph(NodeGraphControl graph)
        {
            foreach (AbstractNode node in _nodes)
            {
                graph.Nodes.Add(node);
            }
            graph.Refresh();
            foreach (Wire connection in _connections)
            {
                graph.Connect(connection.From, connection.To);
            }
            graph.Refresh();
        }

        public void SetSelected(bool selected)
        {
            foreach (AbstractNode node in _nodes)
            {
                node.Selected = selected;
            }
        }
    }
}
