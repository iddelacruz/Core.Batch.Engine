using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Batch.Engine.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Find an <see cref="IAppSession"/> by predicate.
        /// </summary>
        /// <param name="predicate">A predicate delegate.</param>
        /// <returns>The <see cref="IAppSession"/> object or null.</returns>
        Task<IAppSession> FindAsync(Func<IAppSession, bool> predicate);

        /// <summary>
        /// Persist the object in the file.
        /// </summary>
        /// <param name="result">The <see cref="IAppSession"/> object to persist.</param>
        Task PersistAsync(IAppSession session);

        /// <summary>
        /// Delete an object from session storage.
        /// </summary>
        /// <param name="session">The session to be deleted.</param>
        /// <returns>True if it eliminates it, false if not.</returns>
        Task<bool> RemoveAsync(IAppSession session);
    }
}
