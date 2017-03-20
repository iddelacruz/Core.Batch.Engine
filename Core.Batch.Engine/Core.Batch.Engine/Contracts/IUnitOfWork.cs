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
        /// Busca un objeto <see cref="IAppSession"/> por identificador.
        /// </summary>
        /// <param name="identifier">Identificador de la sesión.</param>
        /// <returns>El objeto si lo encuentra, sino devuelve nulo.</returns>
        Task<IAppSession> FindAsync(Guid identifier);

        /// <summary>
        /// Recupera una sesión que ha finalizado incorrectamente.
        /// </summary>
        /// <returns>El objeto si lo encuentra, sino devuelve nulo.</returns>
        Task<IAppSession> RecoverAsync();

        /// <summary>
        /// Persiste el objeto sesión en local.
        /// </summary>
        /// <param name="result">Objeto <see cref="IAppSession"/> a persistir.</param>
        Task PersistAsync(IAppSession result);

        /// <summary>
        /// Elimina un objeto sesión.
        /// </summary>
        /// <param name="identifier">identificador de la sesión a eliminar.</param>
        /// <returns>Verdadero si lo ha eliminado, falso si no.</returns>
        Task<bool> RemoveAsync(Guid identifier);
    }
}
