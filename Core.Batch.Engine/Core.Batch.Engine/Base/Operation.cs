using Core.Batch.Engine.Contracts;
using Core.Batch.Engine.Helpers;
using System;
using System.Threading.Tasks;

namespace Core.Batch.Engine.Base
{
    /// <summary>
    /// Clase base de la que heredarán todas las operaciones a ejecutar dentro de la sesión.
    /// </summary>
    public abstract class Operation : IOperation
    {
        /// <summary>
        /// Identificador de la operación.
        /// </summary>
        public Guid OperationID { get; private set; }

        /// <summary>
        /// Estado de la operación.
        /// </summary>
        public OperationStatus Status { get; set; } = OperationStatus.NotExecuted;

        /// <summary>
        /// Asociación bidireccional entre <see cref="IOperation"/> e <see cref="IAppSession"/>
        /// </summary>
        public IAppSession Session { get; set; }

        /// <summary>
        /// Crea una nueva instancia de <see cref="Operation"/>
        /// </summary>
        public Operation()
        {
            OperationID = Guid.NewGuid();
        }

        /// <summary>
        /// Operación encargada de ejecutar las operaciones que hereden de esta clase.
        /// </summary>
        /// <returns></returns>
        public abstract Task<OperationResponseMessage> SendAsync();
    }
}
