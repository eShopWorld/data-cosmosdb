using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Eshopworld.Data.CosmosDb.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MissingDocumentException : Exception
    {
        public MissingDocumentException()
        {
        }

        public MissingDocumentException(string message) : base(message)
        {
        }

        public MissingDocumentException(string message, Exception inner) : base(message, inner)
        {
        }

        protected MissingDocumentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}
