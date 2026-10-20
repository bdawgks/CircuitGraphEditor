using GraphEditor.JSON;
using GraphEditor.Utils;
using NodeGraphControl;
using NodeGraphControl.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GraphEditor.Nodes.CircuitNodes
{
    internal class JsonParamsConditionalState : JsonParamsData
    {
        public bool StartOn { get; set; }
    }

    [NodeType("ConditionalState",
        JsonParamsType = typeof(JsonParamsConditionalState),
        ContextCategory = "Logic",
        ContextName = "CONDITION")]
    internal class ConditionalStateNode : DisplayParametersNode, ISerializableNode
    {
        private readonly SocketIn _onSocket;
        private readonly SocketIn _offSocket;
        private readonly SocketOut _resultSocket;

        [Category("Parameters")]
        [DisplayParameter]
        public bool StartOn { get; set; }

        public ConditionalStateNode() : this(new Point(0, 0))
        {
        }

        public ConditionalStateNode(Point location)
        {
            Location = location;

            Name = "Conditional State";
            NodeType = GetType().ToString().Split('.').Last();
            Description = "Toggles an output on or off based on independently defined conditions.";
            BaseColor = Color.FromArgb(CommonStates.NodeColorAlpha, 31, 36, 42);
            HeaderColor = Color.DarkMagenta;

            _onSocket = new SocketIn(typeof(CircuitType), "Condition On", this, true);
            _offSocket = new SocketIn(typeof(CircuitType), "Condition Off", this, true);
            _resultSocket = new SocketOut(typeof(CircuitType), "Result", this);
            Sockets.Add(_onSocket);
            Sockets.Add(_offSocket);
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
            return new JsonParamsConditionalState()
            {
                StartOn = StartOn
            };
        }

        void ISerializableNode.SetNodeJsonParams(JsonParamsData nodeJsonParams)
        {
            if (nodeJsonParams is JsonParamsConditionalState jsonParams)
            {
                StartOn = jsonParams.StartOn;
            }
        }
    }
}
