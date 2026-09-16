using GraphEditor.JSON;
using GraphEditor.Nodes;
using NodeGraphControl;
using NodeGraphControl.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestProject.Nodes.CircuitNodes
{
    [NodeType("LogicNot",
        JsonParamsType = typeof(JsonParamsNodeGeneric),
        ParamsTypeName = "Generic",
        ContextCategory = "Logic",
        ContextName = "NOT")]
    public class LogincAndNode : AbstractNode, ISerializableNode
    {
        private readonly SocketIn _inSocket;
        private readonly SocketOut _resultSocket;

        public LogincAndNode() : this(new Point(0, 0))
        {
        }

        public LogincAndNode(Point location)
        {
            Location = location;

            Name = "Logic Not";
            NodeType = GetType().ToString().Split('.').Last();
            Description = "Applies NOT logic to any circuit inputs, and outputs a result circuit.";
            BaseColor = Color.FromArgb(CommonStates.NodeColorAlpha, 31, 36, 42);
            HeaderColor = Color.MidnightBlue;

            _inSocket = new SocketIn(typeof(CircuitType), "Inputs", this, true);
            _resultSocket = new SocketOut(typeof(CircuitType), "Result", this);
            Sockets.Add(_inSocket);
            Sockets.Add(_resultSocket);
        }

        public override bool IsReady()
        {
            return true;
        }

        public override void Execute()
        {
        }

        JsonParamsData ISerializableNode.GetNodeTypeJsonData()
        {
            return new JsonParamsNodeGeneric();
        }

        void ISerializableNode.SetNodeJsonParams(JsonParamsData nodeJsonParams)
        {

        }
    }
}
