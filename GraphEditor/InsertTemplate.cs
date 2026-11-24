using NodeGraphControl.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphEditor
{
    public partial class InsertTemplate : Form
    {
        readonly NodeTemplate _template;
        readonly List<TextBox> _valueControls = new List<TextBox>();
        readonly List<string> _argumentNames = new List<string>();

        public InsertTemplate(NodeTemplate template)
        {
            _template = template;
            InitializeComponent();

            _valueControls.Add(val1);
            if (_template.Arguments.Count > 0)
            {
                for (int i = 0; i < _template.Arguments.Count; i++)
                {
                    if (i > 0)
                    {
                        // Add new label control
                        Label newLabel = new Label
                        {
                            Size = arg1.Size,
                            Left = arg1.Left,
                            Name = string.Format("arg{0}", i + 1),
                            Anchor = arg1.Anchor,
                            Top = arg1.Top + i * (val1.Height + 5)
                        };
                        arg1.Parent.Controls.Add(newLabel);
                        newLabel.Text = _template.Arguments.ToList()[i];
                        newLabel.AutoSize = true;
                        _argumentNames.Add(newLabel.Text);

                        // Add new textbox control
                        TextBox newBox = new TextBox
                        {
                            Size = val1.Size,
                            Left = val1.Left,
                            Name = string.Format("val{0}", i + 1),
                            Anchor = val1.Anchor,
                            Top = val1.Top + i * (val1.Height + 5)
                        };
                        val1.Parent.Controls.Add(newBox);
                        _valueControls.Add(newBox);
                    }
                    else
                    {
                        arg1.Text = _template.Arguments.ToList()[0];
                        _argumentNames.Add(arg1.Text);
                    }
                }
            }
            else
            {
                arg1.Hide();
                val1.Hide();
            }
        }

        public Dictionary<string, string> GetValues()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            int i = 0;
            foreach (var val in _valueControls)
            {
                values.Add(_argumentNames[i], val.Text);
                i++;
            }

            return values;
        }
    }
}
