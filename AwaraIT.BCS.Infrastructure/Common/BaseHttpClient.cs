using System;
using System.Net.Http;

namespace AwaraIT.BCS.Infrastructure.Common
{
    public abstract class BaseHttpClient : IDisposable
    {
        private static readonly object _locker = new object();
        private static volatile HttpClient _client;

        protected static HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    lock (_locker)
                    {
                        if (_client == null)
                        {
                            _client = new HttpClient();
                        }
                    }
                }

                return _client;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client?.Dispose();
                _client = null;
            }
        }
    }
}
