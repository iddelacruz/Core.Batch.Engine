using Core.Batch.Engine.Contracts;
using System;
using System.Threading.Tasks;

namespace Core.Batch.Engine.Base
{
    public sealed class JsonUnitOfWork : IUnitOfWork
    {
        const string fileName = "SessionStorage.json";

        static async Task<IAppSession> Init()
        {
            //TODO: Cargar el fichero de sesión si existe sino crearlo.
            throw new NotImplementedException();
        }

        public Task<IAppSession> FindAsync(Guid identifier)
        {
            //TODO: Buscar dentro del fichero previamente serializado la sesion por identificador.
            throw new NotImplementedException();
        }

        public Task PersistAsync(IAppSession result)
        {
            //TODO: Almacenar los datos en el fichero
            throw new NotImplementedException();
        }

        public Task<IAppSession> RecoverAsync()
        {
            //TODO: Recuperar un elemento del almacenamiento local.
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(Guid identifier)
        {
            //TODO: Eliminar sesiones del almacenamiento local.
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
