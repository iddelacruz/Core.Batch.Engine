using Core.Batch.Engine.Base;
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
        /// Busca una sesión persistida.
        /// </summary>
        /// <param name="predicate">Predicado para buscar la sesión determinada.</param>
        /// <returns>Un objeto <see cref="AppSession"/> en caso de encontrarlo. Nulo si no lo encuentra.</returns>
        Task<AppSession> FindAsync(Func<AppSession, bool> predicate);

        /// <summary>
        /// Persiste un objecto <see cref="AppSession"/>.
        /// </summary>
        /// <param name="session">Objeto a persistir.</param>
        Task PersistAsync(AppSession session);

        /// <summary>
        /// Elimina un elemento <see cref="AppSession"/> previamente persistido.
        /// </summary>
        /// <param name="session"></param>
        Task<bool> RemoveAsync(AppSession session);
    }
}
