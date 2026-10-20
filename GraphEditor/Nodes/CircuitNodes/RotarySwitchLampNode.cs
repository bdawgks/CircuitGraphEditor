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
    internal class JsonParamsRotarySwitchLamp : JsonParamsData
    {
        public string SwitchID { get; set; }
        public int Index { get; set; }
    }

    [NodeType("RotarySwitchLamp",
        JsonParamsType = typeof(JsonParamsRotarySwitchLamp),
        ContextCategory = "Panel Feature")]
    internal class RotarySwitchLampNode : DisplayParametersNode, ISerializableNode
    {
        private readonly SocketIn _inSocket;

        [Category("Parameters")]
        [DisplayParameter]
        public string SwitchID { get; set; } = string.Empty;

        [Category("Parameters")]
        [DisplayParameter]
        public int Index { get; set; }

        public RotarySwitchLampNode() : this(new Point(0, 0))
        {
        }

        public RotarySwitchLampNode(Point location)
        {
            Location = location;

            Name = "Rotary Switch Lamp";
            NodeType = GetType().ToString().Split('.').Last();
            Description = "Activates an indicator lamp on a rotary switch.";
            BaseColor = Color.FromArgb(CommonStates.NodeColorAlpha, 31, 36, 42);
            HeaderColor = Color.DarkCyan;
            NodeWidth = 300;

            _inSocket = new SocketIn(typeof(CircuitType), "Lamp On", this, true);

            Sockets.Add(_inSocket);
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
            return new JsonParamsRotarySwitchLamp()
            {
                SwitchID = SwitchID,
                Index = Index
            };
        }

        public void SetNodeJsonParams(JsonParamsData nodeJsonParams)
        {
            if (nodeJsonParams is JsonParamsRotarySwitchLamp jsonParams)
            {
                SwitchID = jsonParams.SwitchID;
                Index = jsonParams.Index;
            }
        }
    }
}