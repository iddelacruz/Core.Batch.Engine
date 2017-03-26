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
        /// Session identifier.
        /// </summary>
        Guid SessionID { get; }

        /// <summary>
        /// Session state: <see cref="SessionState"/> 
        /// </summary>
        SessionState State { get; set; }

        /// <summary>
        /// Creation date of the session.
        /// </summary>
        DateTime? CreationDate { get; }

        /// <summary>
        /// Date of session update.
        /// </summary>
        DateTime? UpdateDate { get; }

        /// <summary>
        /// List of pending operations.
        /// </summary>
        Queue<IOperation> OperationsRemaining { get; }

        /// <summary>
        /// Listing with the results of the operations performed.
        /// </summary>
        List<IOperation> OperationsResult { get; }

        /// <summary>
        /// Two-way association between <see cref="IAppSession"/> and <see cref="IApplication"/>
        /// </summary>
        IApplication App { get; set; }


        #region Operations
        /// <summary>
        /// Get an object <see cref="IAppSession"/> by identifier.
        /// </summary>
        /// <param name="predicate">A delegate predicate.</param>
        Task GetAsync(Guid identifier);

        /// <summary>
        /// Register operations to the session.
        /// </summary>
        /// <param name="operation">The operation to be added.</param>
        /// <returns>True if added correctly, false if not.</returns>
        Task<bool> RegisterOperationAsync(IOperation operation);

        /// <summary>
        /// Stores changes in memory to each of the operations.
        /// </summary>
        /// <param name="operation">Operation to be stored.</param>
        Task StoreAsync(IOperation operation);

        /// <summary>
        /// Responsible for persisting the session.
        /// </summary>
        Task FlushAsync();
        #endregion

    }
}
