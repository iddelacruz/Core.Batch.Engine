using Core.Batch.Engine.Contracts;
using Core.Batch.Engine.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Batch.Engine.Base
{
    /// <summary>
    /// Unidad de trabajo que se encarga de operar con las sesiones persistidas.
    /// </summary>
    public sealed class JsonUnitOfWork : IUnitOfWork
    {
        const string fileName = "storage.json";

        StorageCollection _storage;

        /// <summary>
        /// Crea una nueva instancia de <see cref="JsonUnitOfWork"/>
        /// </summary>
        public JsonUnitOfWork()
        {
            _storage = new StorageCollection();
        }            

        /// <summary>
        /// Busca una sesión persistida.
        /// </summary>
        /// <param name="predicate">Predicado para buscar la sesión determinada.</param>
        /// <returns>Un objeto <see cref="IAppSession"/> en caso de encontrarlo. Nulo si no lo encuentra.</returns>
        public async Task<IAppSession> FindAsync(Func<IAppSession,bool> predicate)
        {
            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = $"{rootPath}\\{fileName}";
            string json = string.Empty;

            IAppSession session = null;
            if (File.Exists(fullPath))
            {
                StreamReader file = new StreamReader(fullPath);
                try
                {
                    json = await file.ReadToEndAsync();
                    if (!string.IsNullOrEmpty(json))
                    {
                        _storage = JsonConvert.DeserializeObject<StorageCollection>(json, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto,
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        });
                    }
                    session = _storage.Where(predicate).FirstOrDefault();
                    return session;
                }
                catch (Exception)
                {
                    //TODO: LOG
                    throw;
                }
                finally
                {
                    file.Close();
                }
            }
            else
            {
                //TODO: LOG WITH ANY LOG LIBRARY
                return null;
            }
        }

        /// <summary>
        /// Persiste un objecto <see cref="IAppSession"/>.
        /// </summary>
        /// <param name="session">Objeto a persistir.</param>
        public async Task PersistAsync(IAppSession session)
        {
            string json = string.Empty;
            try
            {
                var rootPath = AppDomain.CurrentDomain.BaseDirectory;
                string fullPath = $"{rootPath}\\{fileName}";

                if (File.Exists(fullPath))
                {
                    StreamReader file = new StreamReader(fullPath);
                    json = await file.ReadToEndAsync();
                    if (!string.IsNullOrEmpty(json))
                    {
                        _storage = JsonConvert.DeserializeObject<StorageCollection>(json, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto,
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        });
                    }
                    file.Close();
                }
                _storage.Add(session);

                File.WriteAllText(fullPath, JsonConvert.SerializeObject(_storage, Formatting.Indented,
                    new JsonSerializerSettings {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        TypeNameHandling = TypeNameHandling.Auto
                    }), System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                //TODO: LOG WITH ANY LOG LIBRARY
                throw;
            }

        }

        /// <summary>
        /// Elimina un elemento <see cref="IAppSession"/> previamente persistido.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public Task<bool> RemoveAsync(IAppSession session)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
