using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Eshopworld.Data.CosmosDb.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class StaleDataException : Exception
    {
        public StaleDataException() { }

        protected StaleDataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
