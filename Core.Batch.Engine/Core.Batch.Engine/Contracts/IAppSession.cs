using Core.Batch.Engine.Helpers;
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
        /// Estado de la sesión: <see cref="SessionState"/> 
        /// </summary>
        SessionState State { get; set; }

        /// <summary>
        /// Fecha de creación de la sesión.
        /// </summary>
        DateTime? CreationDate { get; }

        /// <summary>
        /// Fecha de actualización de la sesión.
        /// </summary>
        DateTime? UpdateDate { get; }

        /// <summary>
        /// Listado de las operaciones que quedan pendientes de ejecutar.
        /// </summary>
        Queue<IOperation> OperationsRemaining { get; }

        /// <summary>
        /// Listado con los resultados de las operaciones realizadas.
        /// </summary>
        List<IOperation> OperationsResult { get; }

        /// <summary>
        /// Asociación bidireccional entre <see cref="IAppSession"/>
        /// y <see cref="IApplication"/>
        /// </summary>
        IApplication App { get; set; }


        #region Operations
        /// <summary>
        /// Obtener un objeto <see cref="IAppSession"/> por identififcador.
        /// </summary>
        /// <param name="identifier">Identificador de la sesión.</param>
        Task GetAsync(Guid identifier);

        /// <summary>
        /// Agregar operaciones a la sesión.
        /// </summary>
        /// <param name="operation">La operación que se va a agregar.</param>
        /// <returns>Verdadero si se ha añadido correctamente, falso si no.</returns>
        Task<bool> RegisterOperationAsync(IOperation operation);

        /// <summary>
        /// Almacena en memoria los cambios sobre cada una de las operaciones.
        /// </summary>
        /// <param name="operation">Operación a almacenar.</param>
        Task StoreAsync(IOperation operation);

        /// <summary>
        /// Encargado de persistir la sesión.
        /// </summary>
        Task FlushAsync();
        #endregion

    }
}
