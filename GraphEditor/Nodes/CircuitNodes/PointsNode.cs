using GraphEditor.JSON;
using GraphEditor.Nodes;
using GraphEditor.Utils;
using NodeGraphControl;
using NodeGraphControl.Elements;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace TestProject.Nodes.CircuitNodes
{
    internal class JsonParamsPoints : JsonParamsData
    {
        public string PointsID { get; set; }
    }

    [NodeType("Points",
        JsonParamsType = typeof(JsonParamsPoints),
        ContextCategory = "Track Infrastructure")]
    internal class PointsNode : DisplayParametersNode, ISerializableNode
    {
        private readonly SocketIn _inSocketNormal;
        private readonly SocketOut _outSocketNormal;
        private readonly SocketIn _inSocketReversed;
        private readonly SocketOut _outSocketReversed;

        [Category("Parameters")]
        [DisplayParameter]
        public string PointsID { get; set; }

        public PointsNode() : this(new Point(0, 0))
        {
        }

        public PointsNode(Point location)
        {
            Location = location;

            Name = "Points";
            NodeType = GetType().ToString().Split('.').Last();
            Description = "points control circuit.";
            BaseColor = Color.FromArgb(CommonStates.NodeColorAlpha, 31, 36, 42);
            HeaderColor = Color.Maroon;
            NodeWidth = 250;

            _inSocketNormal = new SocketIn(typeof(CircuitType), "Set Normal", this, true);
            _inSocketReversed = new SocketIn(typeof(CircuitType), "Set Reverse", this, true);
            _outSocketNormal = new SocketOut(typeof(CircuitType), "Normalized", this);
            _outSocketReversed = new SocketOut(typeof(CircuitType), "Reversed", this);

            Sockets.Add(_inSocketNormal);
            Sockets.Add(_inSocketReversed);
            Sockets.Add(_outSocketNormal);
            Sockets.Add(_outSocketReversed);
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
            return new JsonParamsPoints()
            {
                PointsID = PointsID
            };
        }

        public void SetNodeJsonParams(JsonParamsData nodeJsonParams)
        {
            if (nodeJsonParams is JsonParamsPoints signalParams)
            {
                PointsID = signalParams.PointsID;
            }
        }
    }
}
