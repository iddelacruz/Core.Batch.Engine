using Core.Batch.Engine.Base;
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
    /// <summary>
    /// Contrato a implementar por las operaciones.
    /// </summary>
    public interface IOperation : ISignal
    {
        /// <summary>
        /// Identificador de la operación.
        /// </summary>
        Guid OperationID { get; }

        /// <summary>
        /// Estado de la operación.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        OperationState Status { get; set; }

        /// <summary>
        /// Asociación bidireccional entre <see cref="AppSession"/> y <see cref="IOperation"/>
        /// </summary>
        AppSession Session { get; set; }
    }
}
