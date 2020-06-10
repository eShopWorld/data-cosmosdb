using System;
using System.Runtime.Serialization;

namespace Eshopworld.Data.CosmosDb.Exceptions
{
    [Serializable]
    public class StaleDataException : Exception
    {
        public StaleDataException() { }

        protected StaleDataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
