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
    /// Class responsible for containing the operations to be executed.
    /// </summary>
    public sealed class AppSession : IAppSession
    {
        #region Fields
        static IUnitOfWork _unitOfWork;
        #endregion

        #region Properties
        /// <summary>
        /// Session identifier.
        /// </summary>
        public Guid SessionID { get; internal set; }

        /// <summary>
        /// List of pending operations.
        /// </summary>
        public Queue<IOperation> OperationsRemaining { get; internal set; }

        /// <summary>
        /// Listing with the results of the operations performed.
        /// </summary>
        public List<IOperation> OperationsResult { get; internal set; }

        /// <summary>
        /// Creation date of the session.
        /// </summary>
        public DateTime? CreationDate { get; internal set; }

        /// <summary>
        /// Date of session update.
        /// </summary>
        public DateTime? UpdateDate { get; internal set; }

        /// <summary>
        /// Session state: <see cref="SessionState"/> 
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SessionState State { get; set; }

        /// <summary>
        /// Two-way association between <see cref="IAppSession"/> and <see cref="IApplication"/>
        /// </summary>
        public IApplication App { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initialize <see cref="IUnitOfWork"/> instance.
        /// </summary>
        static AppSession()
        {
            _unitOfWork = new JsonUnitOfWork();
        }

        /// <summary>
        /// Create a new instance of <see cref="AppSession"/>
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
        /// Get an object <see cref="IAppSession"/> by identifier.
        /// </summary>
        /// <param name="identifier">Session identifier.</param>
        public async Task GetAsync(Guid identifier)
        {
            await _unitOfWork.FindAsync(x => x.SessionID == identifier);
        }

        /// <summary>
        /// Register operations to the session.
        /// </summary>
        /// <param name="operation">The operation to be added.</param>
        /// <returns>True if added correctly, false if not.</returns>
        public async Task<bool> RegisterOperationAsync(IOperation operation)
        {
            return await Task.Run(() => 
            {
                if ((operation != null) && (!OperationsRemaining.Contains(operation)))
                {
                    OperationsRemaining.Enqueue(operation);
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Stores changes in memory to each of the operations.
        /// </summary>
        /// <param name="operation">Operation to be stored.</param>
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
        /// Responsible for persisting the session.
        /// </summary>
        public async Task FlushAsync()
        {
            await _unitOfWork.PersistAsync(this);
        }

        /// <summary>
        /// Retrieve a persistent session that has status <see cref="SessionState.Uncompleted"/>
        /// </summary>
        /// <returns>The session recovered.</returns>
        public static async Task<IAppSession> RecoverAsync()
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
