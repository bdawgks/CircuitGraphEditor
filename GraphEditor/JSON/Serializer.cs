using NodeGraphControl;
using NodeGraphControl.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using GraphEditor.Nodes;

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

                _nodeJsonParamTypes.Add(nodeTypeAttribute.NodeType, nodeTypeAttribute.JsonParamsType);
            }
        }

        public static bool Save(string filePath, NodeGraphControl.NodeGraphControl graph)
        {
            JsonData data = new JsonData
            {
                Nodes = new List<JsonNodeData>()
            };

            Dictionary<AbstractNode, int> idFromNode = new Dictionary<AbstractNode, int>();
            int idx = 0;
            foreach (AbstractNode node in graph.Nodes)
            {
                idFromNode.Add(node, idx);
                JsonNodeData nodeData = new JsonNodeData()
                {
                    ID = idx,
                    X = node.Location.X,
                    Y = node.Location.Y,
                    Name = node.Name,
                    Description = node.Description
                };

                if (node is ISerializableNode sNode)
                {
                    nodeData.NodeData = sNode.GetNodeTypeJsonData();
                    if (!NodeTypeAttribute.TryGetAttribute(sNode.GetType(), out NodeTypeAttribute nodeTypeAttribute))
                        nodeData.NodeData.NodeType = nodeTypeAttribute.NodeType;
                }

                data.Nodes.Add(nodeData);
                idx++;
            }

            data.Links = new List<JsonLinkData>();
            foreach (Wire wire in graph.Connections)
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
            string fileData = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, fileData);

            return true;
        }

        public static bool Load(string filePath, NodeGraphControl.NodeGraphControl graph)
        {
            string jsonData = File.ReadAllText(filePath);

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                TypeInfoResolver = new JsonNodeParamsTypeResolver()
            };
            JsonData data = JsonSerializer.Deserialize<JsonData>(jsonData, options);

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
                    newNodeObj.Location = new Point(nodeData.X, nodeData.Y);
                    newNodeObj.Name = nodeData.Name;
                    newNodeObj.Description = nodeData.Description;
                    newNodeObj.Calculate();
                    newNodeObj.Execute();

                    if (newNodeObj is ISerializableNode sNode)
                    {
                        sNode.SetNodeJsonParams(nodeData.NodeData);
                    }

                    graph.AddNode(newNodeObj);

                    nodeFromId.Add(nodeData.ID, newNodeObj);
                }
            }

            graph.Refresh();

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
                        graph.Connect(outSocket, inSocket);
                    }
                }

            }

            graph.Refresh();

            return true;
        }
    }
}
