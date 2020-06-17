using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data;

namespace DescLogicFramework
{
    /// <summary>
    /// A Dictionary to store references to objects.
    /// </summary>
    /// <typeparam name="T">The Key Type</typeparam>
    /// <typeparam name="T1">The Value Type</typeparam>
    public class Cache<T, T1> : IEnumerable<T1>, IDisposable
    {
        /// <summary>
        /// The Cache underlying dictionary storage.
        /// </summary>
        private Dictionary<T, T1> data = new Dictionary<T, T1>();

        /// <summary>
        /// Adds a value to the dictionary at a specific key.
        /// </summary>
        /// <param name="key">The dictionary key.</param>
        /// <param name="value"> The value to add.</param>
        public void Add(T key, T1 value)
        {
            this.data[key] = value;
        }
        /// <summary>
        /// Removes a value from the dictionary by referencing a lookup key.
        /// </summary>
        /// <param name="key"></param>
        public void Remove(T key)
        {
            this.data.Remove(key);
        }
        /// <summary>
        /// Returns a value associated with a dictionary key.
        /// </summary>
        /// <param name="key">The key to lookup.</param>
        /// <returns></returns>
        public T1 Get(T key)
        {
            if (this.data.ContainsKey(key))
            {
                return this.data[key];
            }
            return default(T1);
        }

        /// <summary>
        /// Returns the dictionary collection
        /// </summary>
        /// <returns></returns>
        public Dictionary<T, T1> GetCollection()
        {
            return this.data;
        }
        public IEnumerator<T1> GetEnumerator()
        {
            foreach (T key in this.data.Keys)
            {
                yield return Get(key);
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an entire cache to this cache.
        /// </summary>
        /// <param name="CacheToAdd"></param>
        public void AddCache(Cache<T, T1> CacheToAdd)
        {
            foreach(KeyValuePair<T,T1> record in CacheToAdd.GetCollection())
            {
                this.Add(record.Key, record.Value);
            }
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if(data != null)
                    {
                        data = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Cache()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
             GC.SuppressFinalize(this);
        }
        #endregion



    }
}
