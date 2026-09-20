using GraphEditor.Nodes;
using NodeGraphControl;
using NodeGraphControl.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace GraphEditor.JSON
{
    internal class JsonData
    {
        public List<JsonNodeData> Nodes { get; set; }
        public List<JsonLinkData> Links { get; set; }
    }

    internal struct JsonNodeData
    {
        public int ID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Name {  get; set; }
        public string Description { get; set; }
        public JsonParamsData NodeData { get; set; }
    }

    internal struct JsonLinkData
    {
        public int FromID { get; set; }
        public int ToID { get; set; }
        public string FromSocket { get; set; }
        public string ToSocket { get; set; }
    }

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "ParamsType")]
    abstract class JsonParamsData
    {
        public string NodeType { get; set; }
    }
    internal class JsonParamsNodeGeneric : JsonParamsData { }

    internal class JsonNodeParamsTypeResolver : DefaultJsonTypeInfoResolver
    {
        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

            Type paramsType = typeof(JsonParamsData);
            if (jsonTypeInfo.Type == paramsType)
            {
                jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    TypeDiscriminatorPropertyName = "ParamsType",
                    IgnoreUnrecognizedTypeDiscriminators = true,
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization
                };

                foreach (var pair in Serializer.NodeJsonParamTypes)
                {
                    jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(pair.Value, pair.Key));
                }
            }

            return jsonTypeInfo;
        }
    }

    internal class Serializer
    {
        private readonly static Dictionary<string, Type> _serialNodeTypes = new Dictionary<string, Type>();
        private readonly static Dictionary<string, Type> _nodeJsonParamTypes = new Dictionary<string, Type>();

        public static Dictionary<string, Type> NodeJsonParamTypes
        {
            get => _nodeJsonParamTypes;
        }

        public static void InitNodeFactory()
        {
            if (_serialNodeTypes.Count > 0)
                _serialNodeTypes.Clear();

            var typesWithAttribute = NodeTypeAttribute.GetAttributedTypes();

            foreach (var type in typesWithAttribute)
            {
                if (!NodeTypeAttribute.TryGetAttribute(type, out NodeTypeAttribute nodeTypeAttribute))
                    continue;

                if (typeof(ISerializableNode).IsAssignableFrom(type))
                    _serialNodeTypes.Add(nodeTypeAttribute.NodeType, type);

                if (_nodeJsonParamTypes.ContainsKey(nodeTypeAttribute.ParamsTypeName))
                    continue;

                _nodeJsonParamTypes.Add(nodeTypeAttribute.ParamsTypeName, nodeTypeAttribute.JsonParamsType);
            }
        }

        public static bool SerializeSelection(GraphSelection selection, out string serialData)
        {
            serialData = null;

            JsonData data = new JsonData
            {
                Nodes = new List<JsonNodeData>()
            };

            Dictionary<AbstractNode, int> idFromNode = new Dictionary<AbstractNode, int>();
            int idx = 0;
            foreach (AbstractNode node in selection.Nodes)
            {
                idFromNode.Add(node, idx);
                JsonNodeData nodeData = new JsonNodeData()
                {
                    ID = idx,
                    X = node.Location.X - selection.Origin.X,
                    Y = node.Location.Y - selection.Origin.Y,
                    Name = node.Name,
                    Description = node.Description
                };

                if (node is ISerializableNode sNode)
                {
                    nodeData.NodeData = sNode.GetNodeTypeJsonData();
                    if (!NodeTypeAttribute.TryGetAttribute(sNode.GetType(), out NodeTypeAttribute nodeTypeAttribute))
                        continue;

                    nodeData.NodeData.NodeType = nodeTypeAttribute.NodeType;
                }

                data.Nodes.Add(nodeData);
                idx++;
            }

            data.Links = new List<JsonLinkData>();
            foreach (Wire wire in selection.Connections)
            {
                if (idFromNode.TryGetValue(wire.From.Parent, out int fromId)
                    && idFromNode.TryGetValue(wire.To.Parent, out int toId))
                {
                    JsonLinkData linkData = new JsonLinkData()
                    {
                        FromID = fromId,
                        ToID = toId,
                        FromSocket = wire.From.SocketName,
                        ToSocket = wire.To.SocketName,
                    };

                    data.Links.Add(linkData);
                }
            }

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                TypeInfoResolver = new JsonNodeParamsTypeResolver()
            };
            serialData = JsonSerializer.Serialize(data, options);
            return true;
        }

        public static bool DeserializeSelection(string serialData, ref GraphSelection selection)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                TypeInfoResolver = new JsonNodeParamsTypeResolver()
            };
            JsonData data = JsonSerializer.Deserialize<JsonData>(serialData, options);

            if (data == null)
            {
                return false;
            }

            Dictionary<int, AbstractNode> nodeFromId = new Dictionary<int, AbstractNode>();
            foreach (JsonNodeData nodeData in data.Nodes)
            {
                if (_serialNodeTypes.TryGetValue(nodeData.NodeData.NodeType, out Type nodeType))
                {
                    var newNodeObj = (AbstractNode)Activator.CreateInstance(nodeType);
                    newNodeObj.Location = new Point(nodeData.X + selection.Origin.X, nodeData.Y + selection.Origin.Y);
                    newNodeObj.Name = nodeData.Name;
                    newNodeObj.Description = nodeData.Description;
                    newNodeObj.Calculate();
                    newNodeObj.Execute();

                    if (newNodeObj is ISerializableNode sNode)
                    {
                        sNode.SetNodeJsonParams(nodeData.NodeData);
                    }

                    selection.Nodes.Add(newNodeObj);

                    nodeFromId.Add(nodeData.ID, newNodeObj);
                }
            }

            if (data.Links == null)
                return true;

            foreach (JsonLinkData linkData in data.Links)
            {
                if (nodeFromId.TryGetValue(linkData.FromID, out AbstractNode fromNode)
                    && nodeFromId.TryGetValue(linkData.ToID, out AbstractNode toNode))
                {
                    AbstractSocket fromSocket = fromNode.GetSocketByName(linkData.FromSocket);
                    AbstractSocket toSocket = toNode.GetSocketByName(linkData.ToSocket);

                    if (fromSocket is SocketOut outSocket && toSocket is SocketIn inSocket)
                    {
                        Wire wire = new Wire(outSocket, inSocket);
                        selection.Connections.Add(wire);
                    }
                }
            }
            return true;
        }

        public static bool Save(string filePath, NodeGraphControl.NodeGraphControl graph)
        {
            if (SerializeSelection(new GraphSelection(graph), out string fileData))
            {
                File.WriteAllText(filePath, fileData);
                return true;
            }

            return false;
        }

        public static bool Load(string filePath, NodeGraphControl.NodeGraphControl graph)
        {
            string jsonData = File.ReadAllText(filePath);
            GraphSelection selection = new GraphSelection();
            if (DeserializeSelection(jsonData, ref selection))
            {
                graph.Clear();
                selection.AddToGraph(graph);
                return true;
            }

            return false;
        }
    }
}
