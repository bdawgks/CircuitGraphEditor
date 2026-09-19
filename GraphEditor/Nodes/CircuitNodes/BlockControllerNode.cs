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

namespace GraphEditor.Nodes.CircuitNodes
{
    internal class JsonParamsBlockController : JsonParamsData
    {
        public string SignalID { get; set; }
        public string RouteID { get; set; }
    }

    [NodeType("BlockController",
        JsonParamsType = typeof(JsonParamsBlockController),
        ContextCategory = "Track Infrastructure")]
    internal class BlockControllerNode : DisplayParametersNode, ISerializableNode
    {
        private readonly SocketIn _setSocket;
        private readonly SocketIn _unsetSocket;

        [Category("Parameters")]
        [DisplayParameter]
        public string SignalID { get; set; }

        [Category("Parameters")]
        [DisplayParameter]
        public string RouteID { get; set; }

        public BlockControllerNode() : this(new Point(0, 0))
        {
        }

        public BlockControllerNode(Point location)
        {
            Location = location;

            Name = "Block Controller";
            NodeType = GetType().ToString().Split('.').Last();
            Description = "Activates a block controller signal route.";
            BaseColor = Color.FromArgb(CommonStates.NodeColorAlpha, 31, 36, 42);
            HeaderColor = Color.Purple;
            NodeWidth = 250;

            _setSocket = new SocketIn(typeof(CircuitType), "Set Route", this, true);
            _unsetSocket = new SocketIn(typeof(CircuitType), "Unset Route", this, true);

            Sockets.Add(_setSocket);
            Sockets.Add(_unsetSocket);
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
            return new JsonParamsBlockController()
            {
                SignalID = SignalID,
                RouteID = RouteID
            };
        }

        public void SetNodeJsonParams(JsonParamsData nodeJsonParams)
        {
            if (nodeJsonParams is JsonParamsBlockController jsonParams)
            {
                SignalID = jsonParams.SignalID;
                RouteID = jsonParams.RouteID;
            }
        }
    }
}