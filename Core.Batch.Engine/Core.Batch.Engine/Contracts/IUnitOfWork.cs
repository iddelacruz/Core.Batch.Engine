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
        /// <returns>Un objeto <see cref="IAppSession"/> en caso de encontrarlo. Nulo si no lo encuentra.</returns>
        Task<IAppSession> FindAsync(Func<IAppSession, bool> predicate);

        /// <summary>
        /// Persiste un objecto <see cref="IAppSession"/>.
        /// </summary>
        /// <param name="session">Objeto a persistir.</param>
        Task PersistAsync(IAppSession session);

        /// <summary>
        /// Elimina un elemento <see cref="IAppSession"/> previamente persistido.
        /// </summary>
        /// <param name="session"></param>
        Task<bool> RemoveAsync(IAppSession session);
    }
}
