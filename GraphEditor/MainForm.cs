using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using NodeGraphControl.Elements;
using TestProject.JSON;
using TestProject.Nodes;
using TestProject.Nodes.CircuitNodes;
using TestProject.Nodes.MathNodes;

namespace TestProject {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e) 
        {
            Serializer.InitNodeFactory();

            // CreateTestNodes();
            
            // add nodes to control menu context
            nodeGraphControl.AddContextNodeType<NodeString>("Sample String", "This node provides sample string", "Generators");
            nodeGraphControl.AddContextNodeType<NodeStringXOR>("String XOR node", "This node encodes string with XOR function", "Operators");
            nodeGraphControl.AddContextNodeType<NodeChar>("Sample Char", "This node provides char", "Generators");
            nodeGraphControl.AddContextNodeType<NodeLogicOutput>("Logic Output", "This node provides binary output", "Generators");
            nodeGraphControl.AddContextNodeType<NodeMonitor>("Monitor", "ToString()", "");

            nodeGraphControl.AddContextNodeType<MathSumNode>("Math Sum", "Sums two integers", "Math");
            nodeGraphControl.AddContextNodeType<MathNumNode>("Math Num", "Generates an integer", "Math");
            nodeGraphControl.AddContextNodeType<MathNumNode>("Math Num", "Generates an integer", "Generators");
            nodeGraphControl.AddContextNodeType<MathAvgNode>("Math Avg", "This node calculates the average of the numbers on the input" , "Math");

            nodeGraphControl.AddContextNodeType<RotarySwitchNode>("Rotary Switch", "ToString()" , "Circuits");
            nodeGraphControl.AddContextNodeType<StateButtonNode>("State Button", "ToString()" , "Circuits");

            nodeGraphControl.AddContextNodeType<LogincAndNode>("Circuit And", "ToString()" , "Circuits");
            
            // set type colors
            nodeGraphControl.AddTypeColorPair<double>(Color.DodgerBlue);
            nodeGraphControl.AddTypeColorPair<bool>(Color.LightGray);
            
            // run
            nodeGraphControl.Run();
        }

        private void CreateTestNodes() {
            // create test nodes
            var testNode = new NodeStringXOR(new Point(100, 50));
            var testNode2 = new NodeString(new Point(-350, 40));
            var testNode3 = new NodeChar(new Point(-300, -150));
            var testNode4 = new NodeStringXOR(new Point(350, 150));
            var testNode5 = new NodeLogicOutput(new Point(-280, 300));
            var testNode6 = new NodeMonitor(new Point(600, 290));

            // add nodes to control
            nodeGraphControl.AddNode(testNode);
            nodeGraphControl.AddNode(testNode2);
            nodeGraphControl.AddNode(testNode3);
            nodeGraphControl.AddNode(testNode4);
            nodeGraphControl.AddNode(testNode5);
            nodeGraphControl.AddNode(testNode6);

            // connect nodes
            nodeGraphControl.Connect((SocketOut) testNode3.Sockets[0], (SocketIn) testNode.GetSocketByName("Input key"));
            nodeGraphControl.Connect((SocketOut) testNode2.Sockets[0], (SocketIn) testNode.GetSocketByName("Input string"));
            nodeGraphControl.Connect((SocketOut) testNode.GetSocketByName("Output string"), (SocketIn) testNode4.GetSocketByName("Input string"));
            nodeGraphControl.Connect((SocketOut) testNode3.Sockets[0], (SocketIn) testNode4.GetSocketByName("Input key"));
            nodeGraphControl.Connect((SocketOut) testNode5.GetSocketByName("Output H"), (SocketIn) testNode.GetSocketByName("Input enabled"));
            nodeGraphControl.Connect((SocketOut) testNode5.GetSocketByName("Output H"), (SocketIn) testNode4.GetSocketByName("Input enabled"));
            nodeGraphControl.Connect((SocketOut) testNode4.GetSocketByName("Output string"), (SocketIn) testNode6.GetSocketByName("Input"));
        }

        private void NodeGraph_SelectionChanged(object sender, List<AbstractNode> abstractNodes) {
            if (abstractNodes.Count == 1) {
                var node = abstractNodes[0];
                var nodeType = node.GetType();
                dynamic changedObj = Convert.ChangeType(node, nodeType);
                propertyGrid.SelectedObject = changedObj;
            } else {
                propertyGrid.SelectedObject = null;
            }
        }

        private void nodeGraphControl_Paint(object sender, PaintEventArgs e) {
            propertyGrid.Refresh();
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
            AbstractNode node = propertyGrid.SelectedObject as AbstractNode;
            node?.Execute();
        }
        
        private void nodeGraphControl_ZoomChanged(object sender, float e) {
            statusBarPanelZoom.Text = "(Zoom) " + e.ToString("F",CultureInfo.InvariantCulture) + "x";    
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Application.LocalUserAppDataPath;
            saveFileDialog.Filter = "json files(*.json) | *.json";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Serializer.Save(saveFileDialog.FileName, nodeGraphControl);
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Application.LocalUserAppDataPath;
            openFileDialog.Filter = "json files(*.json) | *.json";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Serializer.Load(openFileDialog.FileName, nodeGraphControl);
            }
        }
    }
}