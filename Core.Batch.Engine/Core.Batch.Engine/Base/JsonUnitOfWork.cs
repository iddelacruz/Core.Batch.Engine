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
    public sealed class JsonUnitOfWork : IUnitOfWork
    {
        const string fileName = "storage.json";
        StorageCollection _storage;

        public JsonUnitOfWork()
        {
            _storage = new StorageCollection();
        }            

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
