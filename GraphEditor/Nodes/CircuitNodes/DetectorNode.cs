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
using TestProject.Nodes.CircuitNodes;

namespace GraphEditor.Nodes.CircuitNodes
{
    internal class JsonParamsDetector : JsonParamsData
    {
        public string DetectorID { get; set; }
    }

    [NodeType("Detector",
        JsonParamsType = typeof(JsonParamsDetector),
        ContextCategory = "Track Infrastructure")]
    internal class DetectorNode : DisplayParametersNode, ISerializableNode
    {
        private readonly SocketOut _outSocketOccupied;

        [Category("Parameters")]
        [DisplayParameter]
        public string DetectorID { get; set; }

        public DetectorNode() : this(new Point(0, 0))
        {
        }

        public DetectorNode(Point location)
        {
            Location = location;

            Name = "Detector";
            NodeType = GetType().ToString().Split('.').Last();
            Description = "Detector control circuit.";
            BaseColor = Color.FromArgb(CommonStates.NodeColorAlpha, 31, 36, 42);
            HeaderColor = Color.SaddleBrown;
            NodeWidth = 250;

            _outSocketOccupied = new SocketOut(typeof(CircuitType), "Occupied", this);

            Sockets.Add(_outSocketOccupied);
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
            return new JsonParamsDetector()
            {
                DetectorID = DetectorID
            };
        }

        public void SetNodeJsonParams(JsonParamsData nodeJsonParams)
        {
            if (nodeJsonParams is JsonParamsDetector detectorParams)
            {
                DetectorID = detectorParams.DetectorID;
            }
        }
    }
}
