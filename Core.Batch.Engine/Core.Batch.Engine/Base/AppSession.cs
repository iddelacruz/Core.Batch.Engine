using Core.Batch.Engine.Contracts;
using Core.Batch.Engine.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Batch.Engine.Base
{
    /// <summary>
    /// Clase que contendrá las operaciones a ejecutar.
    /// </summary>
    public sealed class AppSession
    {
        #region Fields
        static IUnitOfWork _unitOfWork;
        #endregion

        #region Properties
        /// <summary>
        /// Identificador de la sesión.
        /// </summary>
        public Guid SessionID { get; internal set; }

        /// <summary>
        /// Listado de operaciones pendientes.
        /// </summary>
        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.Auto)]
        public Queue<IOperation> OperationsRemaining { get; internal set; }

        /// <summary>
        /// Lista con los resultados de las operaciones realizadas.
        /// </summary>
        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.Auto)]
        public List<IOperation> OperationsResult { get; internal set; }

        /// <summary>
        /// Fecha de creación de la sesion.
        /// </summary>
        public DateTime? CreationDate { get; internal set; }

        /// <summary>
        /// Fecha de actualización de la sesión.
        /// </summary>
        public DateTime? UpdateDate { get; internal set; }

        /// <summary>
        /// Propiedad que determina almacena el estado de la sesión.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SessionState State { get; set; }

        /// <summary>
        /// Asociación bidireccional entre <see cref="AppSession"/> y <see cref="Application"/>
        /// </summary>
        [JsonIgnore]
        public Application App { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa una vez una implementación de <see cref="IUnitOfWork"/>.
        /// </summary>
        static AppSession()
        {
            _unitOfWork = new JsonUnitOfWork();
        }

        /// <summary>
        /// Crea una nueva instancia de <see cref="AppSession"/>
        /// </summary>
        public AppSession()
        {
            SessionID = Guid.NewGuid();
            OperationsRemaining = new Queue<IOperation>();
            State = SessionState.Initial;
            CreationDate = DateTime.Now;
        }
        #endregion

        #region Operations
        /// <summary>
        /// Obtiene un objeto <see cref="AppSession"/> por su identificador.
        /// </summary>
        /// <param name="identifier">Session identifier.</param>
        public async Task GetAsync(Guid identifier)
        {
            await _unitOfWork.FindAsync(x => x.SessionID == identifier);
        }

        /// <summary>
        /// Registra las operaciones en la sesión.
        /// </summary>
        /// <param name="operation">Operación a registrar.</param>
        /// <returns>Verdadero si se ha añadido correctamente, falso si no.<returns>
        public async Task<bool> RegisterOperationAsync(IOperation operation)
        {
            return await Task.Run(() => 
            {
                if ((operation != null) && (!OperationsRemaining.Contains(operation)))
                {
                    OperationsRemaining.Enqueue(operation);
                    operation.Session = this;
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Almacena en memoria los cambios en cada una de las operaciones.
        /// </summary>
        /// <param name="operation">Operación a almacenar.</param>
        public async Task StoreAsync(IOperation operation)
        {
            await Task.Run(() => 
            {
                if (operation == null)
                {
                    throw new ArgumentNullException("operation");
                }

                if (OperationsResult == null)
                {
                    OperationsResult = new List<IOperation>();
                    OperationsResult.Add(operation);
                }
                else
                {
                    var index = OperationsResult
                    .IndexOf
                    (OperationsResult
                    .Where(x => x.OperationID.Equals(operation.OperationID))
                    .FirstOrDefault());

                    if (index == -1)
                    {
                        OperationsResult.Add(operation);
                    }
                    else
                    {
                        OperationsResult[index] = operation;
                        UpdateDate = DateTime.Now;
                    }
                }
            });
        }

        /// <summary>
        /// Se encarga de persistir la sesión.
        /// </summary>
        public async Task FlushAsync()
        {
            await _unitOfWork.PersistAsync(this);
        }

        /// <summary>
        /// Recupera una sesión que ha sido persistida anteriormente. 
        /// El estado de la sesión debe ser: <see cref="SessionState.Uncompleted"/>
        /// </summary>
        /// <returns>The session recovered.</returns>
        public static async Task<AppSession> RecoverAsync()
        {
            return await _unitOfWork.FindAsync(x => x.State == SessionState.Uncompleted);
        }

        #region Dispose pattern
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_unitOfWork != null)
                {
                    _unitOfWork.Dispose();
                }
            }
        }
        #endregion
        #endregion
    }
}
