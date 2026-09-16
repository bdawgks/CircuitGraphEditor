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
    internal class JsonParamsSignalAspect : JsonParamsData
    {
        public string SignalID { get; set; }
        public string Aspect { get; set; }
    }

    [NodeType("SignalAspect",
        JsonParamsType = typeof(JsonParamsSignalAspect),
        ContextCategory = "Track Infrastructure")]
    internal class SignalAspectNode : DisplayParametersNode, ISerializableNode
    {
        private readonly SocketIn _inSocket;
        private readonly SocketOut _outSocket;

        [Category("Parameters")]
        [DisplayParameter]
        public string SignalID { get; set; }

        [Category("Parameters")]
        [DisplayParameter]
        public string Aspect { get; set; } = string.Empty;

        public SignalAspectNode() : this(new Point(0, 0))
        {
        }

        public SignalAspectNode(Point location)
        {
            Location = location;

            Name = "Signal Aspect";
            NodeType = GetType().ToString().Split('.').Last();
            Description = "Signal aspect control circuit.";
            BaseColor = Color.FromArgb(CommonStates.NodeColorAlpha, 31, 36, 42);
            HeaderColor = Color.DimGray;
            NodeWidth = 300;

            _inSocket = new SocketIn(typeof(CircuitType), "Set Aspect", this, true);
            _outSocket = new SocketOut(typeof(CircuitType), "Aspect Shown", this);

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
            return new JsonParamsSignalAspect()
            {
                SignalID = SignalID,
                Aspect = Aspect
            };
        }

        public void SetNodeJsonParams(JsonParamsData nodeJsonParams)
        {
            if (nodeJsonParams is JsonParamsSignalAspect signalParams)
            {
                SignalID = signalParams.SignalID;
                Aspect = signalParams.Aspect;
            }
        }
    }
}
