using Core.Batch.Engine.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Core.Batch.Engine.Helpers
{
    public sealed class StorageCollection : ICollection<IAppSession>
    {
        IList<IAppSession> _helperList = new List<IAppSession>();

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

        public void Add(IAppSession item)
        {
            if (_helperList.Contains(item))
            {
                _helperList.Remove(item);
            }
            _helperList.Add(item);
        }

        public void Clear()
        {
            _helperList.Clear();
        }

        public bool Contains(IAppSession item)
        {
            return _helperList.Contains(item);
        }

        public void CopyTo(IAppSession[] array, int arrayIndex)
        {
            _helperList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IAppSession> GetEnumerator()
        {
            return _helperList.GetEnumerator();
        }

        public bool Remove(IAppSession item)
        {
            return _helperList.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
