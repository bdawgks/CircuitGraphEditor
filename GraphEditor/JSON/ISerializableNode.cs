using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.JSON
{
    internal interface ISerializableNode
    {
        JsonParamsData GetNodeTypeJsonData();

        void SetNodeJsonParams(JsonParamsData nodeJsonParams);
    }
}
