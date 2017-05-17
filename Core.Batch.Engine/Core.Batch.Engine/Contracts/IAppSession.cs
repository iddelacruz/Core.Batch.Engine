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
    public interface IAppSession : IDisposable
    {
        /// <summary>
        /// Identificador de la sesión.
        /// </summary>
        Guid SessionID { get; }

        /// <summary>
        /// Propiedad que determina almacena el estado de la sesión.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        SessionState State { get; set; }

        /// <summary>
        /// Fecha de creación de la sesion.
        /// </summary>
        DateTime? CreationDate { get; }

        /// <summary>
        /// Fecha de actualización de la sesión.
        /// </summary>
        DateTime? UpdateDate { get; }

        /// <summary>
        /// Listado de operaciones pendientes.
        /// </summary>
        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.Auto)]
        Queue<IOperation> OperationsRemaining { get; }

        /// <summary>
        /// Lista con los resultados de las operaciones realizadas.
        /// </summary>
        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.Auto)]
        List<IOperation> OperationsResult { get; }

        /// <summary>
        /// Asociación bidireccional entre <see cref="IAppSession"/> y <see cref="IApplication"/>
        /// </summary>
        [JsonIgnore]
        IApplication App { get; set; }


        #region Operations
        /// <summary>
        /// Obtiene un objeto <see cref="IAppSession"/> por su identificador.
        /// </summary>
        /// <param name="identifier">Session identifier.</param>
        Task GetAsync(Guid identifier);

        /// <summary>
        /// Registra las operaciones en la sesión.
        /// </summary>
        /// <param name="operation">Operación a registrar.</param>
        /// <returns>Verdadero si se ha añadido correctamente, falso si no.<returns>
        Task<bool> RegisterOperationAsync(IOperation operation);

        /// <summary>
        /// Almacena en memoria los cambios en cada una de las operaciones.
        /// </summary>
        /// <param name="operation">Operación a almacenar.</param>
        Task StoreAsync(IOperation operation);

        /// <summary>
        /// Se encarga de persistir la sesión.
        /// </summary>
        Task FlushAsync();
        #endregion

    }
}
