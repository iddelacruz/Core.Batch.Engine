using Core.Batch.Engine.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Core.Batch.Engine.Base;

namespace Core.Batch.Engine.Helpers
{
    public sealed class StorageCollection : ICollection<AppSession>
    {
        IList<AppSession> _helperList = new List<AppSession>();

        public int Count
        {
            get
            {
                return _helperList.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _helperList.IsReadOnly;
            }
        }

        public void Add(AppSession item)
        {
            if (_helperList.Contains(item))
            {
                _helperList.Remove(item);
            }

            var sessionElement = _helperList
                .Where(x => x.State == SessionState.Uncompleted)
                .SingleOrDefault();

            if (sessionElement != null)
            {
                _helperList.Remove(sessionElement);
            }

            _helperList.Add(item);
        }

        public void Clear()
        {
            _helperList.Clear();
        }

        public bool Contains(AppSession item)
        {
            return _helperList.Contains(item);
        }

        public void CopyTo(AppSession[] array, int arrayIndex)
        {
            _helperList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<AppSession> GetEnumerator()
        {
            return _helperList.GetEnumerator();
        }

        public bool Remove(AppSession item)
        {
            return _helperList.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
