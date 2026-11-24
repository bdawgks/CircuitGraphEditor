using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NodeGraphControl.Elements
{
    public class InsertTemplateEventArgs : EventArgs
    {
        public NodeTemplate Template { get; }
        public Point Point { get; }

        public InsertTemplateEventArgs(NodeTemplate template, Point point)
        {
            Template = template;
            Point = point;
        }
    }

    public class NodeTemplate
    {
        readonly string _name;
        readonly string _json;
        readonly HashSet<string> _arguments = new HashSet<string>();

        public string Name { get => _name; }

        public HashSet<string> Arguments { get => _arguments; }

        public NodeTemplate(string name, string jsonStr)
        {
            _name = name;
            _json = jsonStr;

            MatchCollection matches = Regex.Matches(_json, "(?<=%).*?(?=%)");
            foreach (Match match in matches)
            {
                _arguments.Add(match.Value);
            }
        }

        public bool FormatTemplate(Dictionary<string, string> values, out string json)
        {
            json = _json;

            foreach (var pair in values)
            {
                string arg = string.Format("%{0}%", pair.Key);
                json = json.Replace(arg, pair.Value);
            }

            return true;
        }
    }
}
