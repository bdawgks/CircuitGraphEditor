using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using NodeGraphControl.Elements;
using GraphEditor.JSON;
using GraphEditor.Nodes.CircuitNodes;
using GraphEditor.Nodes;
using System.Reflection;

namespace GraphEditor {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e) 
        {
            Serializer.InitNodeFactory();

            // add nodes to control menu context
            var attributedNodeTypes = NodeTypeAttribute.GetAttributedTypes();
            foreach (var nodeType in attributedNodeTypes)
            {
                if (!NodeTypeAttribute.TryGetAttribute(nodeType, out NodeTypeAttribute attrib))
                    continue;

                string methodName = nameof(NodeGraphControl.NodeGraphControl.AddContextNodeType);
                MethodInfo methodInfo = nodeGraphControl.GetType().GetMethod(methodName);
                MethodInfo genericMethod = methodInfo.MakeGenericMethod(nodeType);
                genericMethod.Invoke(nodeGraphControl, new object[] { attrib.ContextName, "ToString()", attrib.ContextCategory });
            }
            
            // set type colors
            nodeGraphControl.AddTypeColorPair<CircuitType>(Color.GreenYellow);
            
            // run
            nodeGraphControl.Run();
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