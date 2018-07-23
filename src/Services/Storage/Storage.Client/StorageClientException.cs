using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Storage.Client
{
    public class StorageClientException : Exception
    {
        public StorageClientException() { }
        public StorageClientException(string message) : base(message) { }
        public StorageClientException(string message, Exception inner) : base(message, inner) { }
    }
}
