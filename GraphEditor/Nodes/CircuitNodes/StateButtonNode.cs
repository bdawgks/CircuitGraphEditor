using GraphEditor.JSON;
using GraphEditor.Nodes;
using GraphEditor.Utils;
using NodeGraphControl;
using NodeGraphControl.Elements;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace GraphEditor.Nodes.CircuitNodes
{
    internal class JsonParamsStateButtonNode : JsonParamsData
    {
        public string ButtonID { get; set; }
        public int States { get; set; }
    }

    [NodeType("StateButton", 
        JsonParamsType = typeof(JsonParamsStateButtonNode),
        ContextCategory = "Panel Feature")]
    internal class StateButtonNode : DisplayParametersNode, ISerializableNode
    {
        private int _statesCount = 0;

        private readonly ResizableSocket<SocketIn, CircuitType> _inSockets;
        private readonly ResizableSocket<SocketOut, CircuitType> _outSockets;

        [Category("Parameters")]
        [DisplayParameter]
        public string ButtonID { get; set; }

        [Category("Parameters")]
        public int NumberOfStates
        {
            get => _statesCount;
            set => SetStateCount(value);
        }

        public StateButtonNode() : this(new Point(0, 0))
        {
        }

        public StateButtonNode(Point location)
        {
            Location = location;

            Name = "State Button";
            NodeType = GetType().ToString().Split('.').Last();
            Description = "State button circuit control.";
            BaseColor = Color.FromArgb(CommonStates.NodeColorAlpha, 31, 36, 42);
            HeaderColor = Color.DarkSlateBlue;
            NodeWidth = 300;

            _statesCount = 2;
            _inSockets = new ResizableSocket<SocketIn, CircuitType>(this, "Show State #{0}", _statesCount);
            _outSockets = new ResizableSocket<SocketOut, CircuitType>(this, "Activated State #{0}", _statesCount);
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
            _inSockets.Size = _statesCount;
            _outSockets.Size = _statesCount;

            Calculate();
            OnInvokeRepaint(EventArgs.Empty);
        }

        public JsonParamsData GetNodeTypeJsonData()
        {
            return new JsonParamsStateButtonNode()
            {
                ButtonID = ButtonID,
                States = NumberOfStates
            };
        }

        public void SetNodeJsonParams(JsonParamsData nodeJsonParams)
        {
            if (nodeJsonParams is JsonParamsStateButtonNode stateButtonParams)
            {
                ButtonID = stateButtonParams.ButtonID;
                NumberOfStates = stateButtonParams.States;
            }
        }
    }
}
