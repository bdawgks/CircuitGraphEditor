using GraphEditor.JSON;
using GraphEditor.Nodes;
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

namespace TestProject.Nodes.CircuitNodes
{
    internal class JsonParamsSignalIndication : JsonParamsData
    {
        public string SignalID { get; set; }
        public string Indication { get; set; }
    }

    [NodeType("SignalIndication",
        JsonParamsType = typeof(JsonParamsSignalIndication),
        ContextCategory = "Track Infrastructure")]
    internal class SignalIndicationNode : DisplayParametersNode, ISerializableNode
    {
        private readonly SocketIn _inSocket;
        private readonly SocketOut _outSocket;

        [Category("Parameters")]
        [DisplayParameter]
        public string SignalID { get; set; }

        [Category("Parameters")]
        [DisplayParameter]
        public string Indication { get; set; } = string.Empty;

        public SignalIndicationNode() : this(new Point(0, 0))
        {
        }

        public SignalIndicationNode(Point location)
        {
            Location = location;

            Name = "Signal Indication";
            NodeType = GetType().ToString().Split('.').Last();
            Description = "Signal indication control circuit.";
            BaseColor = Color.FromArgb(CommonStates.NodeColorAlpha, 31, 36, 42);
            HeaderColor = Color.DimGray;
            NodeWidth = 300;

            _inSocket = new SocketIn(typeof(CircuitType), "Set Indication", this, true);
            _outSocket = new SocketOut(typeof(CircuitType), "Indication Shown", this);

            Sockets.Add(_inSocket); 
            Sockets.Add(_outSocket);
        }

        public override bool IsReady()
        {
            return true;
        }

        public override void Execute()
        {
        }

        public JsonParamsData GetNodeTypeJsonData()
        {
            return new JsonParamsSignalIndication()
            {
                SignalID = SignalID,
                Indication = Indication
            };
        }

        public void SetNodeJsonParams(JsonParamsData nodeJsonParams)
        {
            if (nodeJsonParams is JsonParamsSignalIndication signalParams)
            {
                SignalID = signalParams.SignalID;
                Indication = signalParams.Indication;
            }
        }
    }
}
