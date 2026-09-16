using GraphEditor.JSON;
using GraphEditor.Utils;
using NodeGraphControl;
using NodeGraphControl.Elements;
using System;
using System.CodeDom;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace GraphEditor.Nodes.CircuitNodes
{
    internal class JsonParamsRotarySwitchNode : JsonParamsData
    {
        public string SwitchID { get; set; }
        public int States { get; set; }
    }

    [NodeType("RotarySwitch", 
        JsonParamsType = typeof(JsonParamsRotarySwitchNode), 
        ContextCategory = "Panel Feature")]
    internal class RotarySwitchNode : DisplayParametersNode, ISerializableNode
    {
        private readonly SocketIn _lockCircuit;
        private int _statesCount = 0;

        private readonly ResizableSocket<SocketOut, CircuitType> _outSockets;

        [Category("Parameters")]
        [DisplayParameter]
        public string SwitchID { get; set; }

        [Category("Parameters")]
        public int NumberOfStates
        {
            get => _statesCount;
            set => SetStateCount(value);
        }
        public RotarySwitchNode() : this(new Point(0, 0))
        {
        }

        public RotarySwitchNode(Point location)
        {
            Location = location;

            _lockCircuit = new SocketIn(typeof(CircuitType), "Interlock", this, true);

            Name = "Rotary Switch";
            NodeType = GetType().ToString().Split('.').Last();
            Description = "Rotary switch circuit control.";
            BaseColor = Color.FromArgb(CommonStates.NodeColorAlpha, 31, 36, 42);
            HeaderColor = Color.DarkSlateGray;

            Sockets.Add(_lockCircuit);
            _statesCount = 2;
            _outSockets = new ResizableSocket<SocketOut, CircuitType>(this, "State {0}", _statesCount);
        }
        public override bool IsReady()
        {
            return true;
        }

        public override void Execute()
        {
        }

        private void SetStateCount(int count)
        {
            if (count == _statesCount)
                return;

            _statesCount = count;
            _outSockets.Size = _statesCount;

            Calculate();
            OnInvokeRepaint(EventArgs.Empty);
        }

        public JsonParamsData GetNodeTypeJsonData()
        {
            return new JsonParamsRotarySwitchNode()
            {
                SwitchID = SwitchID,
                States = NumberOfStates
            };
        }

        public void SetNodeJsonParams(JsonParamsData nodeJsonParams)
        {
            if (nodeJsonParams is JsonParamsRotarySwitchNode rotarySwitchParams)
            {
                SwitchID = rotarySwitchParams.SwitchID;
                NumberOfStates = rotarySwitchParams.States;
            }
        }
    }
}
