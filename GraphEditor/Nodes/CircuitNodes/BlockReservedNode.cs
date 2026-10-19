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
    internal class JsonParamsBlockReserved : JsonParamsData
    {
        public string BlockID { get; set; }
    }

    [NodeType("BlockReserved",
        JsonParamsType = typeof(JsonParamsBlockReserved),
        ContextCategory = "Track Infrastructure")]

    internal class BlockReservedNode : DisplayParametersNode, ISerializableNode
    {
        private readonly SocketOut _reservedSocket;

        [Category("Parameters")]
        [DisplayParameter]
        public string BlockID { get; set; }

        public BlockReservedNode() : this(new Point(0, 0))
        {
        }

        public BlockReservedNode(Point location)
        {
            Location = location;

            Name = "Block Reserved";
            NodeType = GetType().ToString().Split('.').Last();
            Description = "Activates a circuit when a particular block controller block is reserved.";
            BaseColor = Color.FromArgb(CommonStates.NodeColorAlpha, 31, 36, 42);
            HeaderColor = Color.DarkOliveGreen;
            NodeWidth = 250;

            _reservedSocket = new SocketOut(typeof(CircuitType), "Block Reserved", this);

            Sockets.Add(_reservedSocket);
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
            return new JsonParamsBlockReserved()
            {
                BlockID = BlockID
            };
        }

        public void SetNodeJsonParams(JsonParamsData nodeJsonParams)
        {
            if (nodeJsonParams is JsonParamsBlockReserved jsonParams)
            {
                BlockID = jsonParams.BlockID;
            }
        }
    }
}
