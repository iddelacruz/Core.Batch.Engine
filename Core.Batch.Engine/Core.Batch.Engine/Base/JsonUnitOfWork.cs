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
            string fullPath = $"~/Storage/{fileName}";
            string json = string.Empty;
            IAppSession session = null;
            if (File.Exists(fullPath))
            {
                StreamReader file = new StreamReader($"~/Storage/{fileName}");
                try
                {
                    json = await file.ReadToEndAsync();
                    _storage = JsonConvert.DeserializeObject<StorageCollection>(json);
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
                //TODO: LOG
                return null;
            }
        }

        public async Task PersistAsync(IAppSession session)
        {
            string fullPath = $"~/Storage/{fileName}";
            string json = string.Empty;
            try
            {
                if (File.Exists(fullPath))
                {
                    StreamReader file = new StreamReader($"~/Storage/{fileName}");
                    json = await file.ReadToEndAsync();
                    _storage = JsonConvert.DeserializeObject<StorageCollection>(json);
                }
                _storage.Add(session);
                File.WriteAllText(fullPath, JsonConvert.SerializeObject(_storage));
            }
            catch (Exception)
            {
                //TODO: LOG
                throw;
            }

        }

        public Task<bool> RemoveAsync(IAppSession session)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
