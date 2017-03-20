using Core.Batch.Engine.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Batch.Engine.Contracts
{
    public interface IOperation : ISignal
    {
        Guid OperationID { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        OperationStatus Status { get; set; }

    }
}
