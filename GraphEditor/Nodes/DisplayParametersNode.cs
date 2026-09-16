using GraphEditor.Utils;
using NodeGraphControl.Elements;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace GraphEditor.Nodes
{
    internal abstract class DisplayParametersNode : AbstractNode
    {
        private readonly List<PropertyInfo> _properties = new List<PropertyInfo>();
        private readonly List<DisplayParameterAttribute> _attributes = new List<DisplayParameterAttribute>();

        private readonly Font _font = new Font(FontFamily.GenericMonospace, 10f, FontStyle.Regular);
        private const int _rowHeight = 10;
        private const int _rowSpace = 5;

        protected DisplayParametersNode()
        {
            foreach (var property in GetType().GetProperties())
            {
                foreach (var attrib in property.GetCustomAttributes())
                {
                    if (attrib is DisplayParameterAttribute displayAttrib)
                    {
                        _properties.Add(property);
                        _attributes.Add(displayAttrib);
                        break;
                    }
                }
            }

            int count = _properties.Count;
            FooterHeight = count * _rowHeight + (count + 1) * _rowSpace;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override void Draw(Graphics g)
        {
            base.Draw(g);

            for (int i = 0; i < _properties.Count; i++)
            {
                string property = _properties[i].Name;
                var value = _properties[i].GetValue(this);
                string valueStr = "";
                if (value != null)
                    valueStr = value.ToString();

                string formatStr = string.Format("{0}: {1}", property, valueStr);

                PointF pos = new PointF()
                {
                    X = Location.X + 3,
                    Y = Location.Y + FullHeight - FooterHeight + (_rowHeight * i) + (_rowSpace * i)
                };
                g.DrawString(formatStr, _font, new SolidBrush(Color.MediumOrchid), pos);
            }
        }
    }
}
