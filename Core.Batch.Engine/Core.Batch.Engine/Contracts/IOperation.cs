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
        OperationStatus Status { get; set; }

        /// <summary>
        /// Asociación bidireccional entre <see cref="IAppSession"/> y <see cref="IOperation"/>
        /// </summary>
        IAppSession Session { get; set; }
    }
}
