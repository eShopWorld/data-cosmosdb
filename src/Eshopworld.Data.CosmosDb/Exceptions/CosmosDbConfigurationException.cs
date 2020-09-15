using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Eshopworld.Data.CosmosDb.Exceptions
{
    /// <summary>
    /// Exception for when CosmosDBConfiguration is incorrect
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class CosmosDbConfigurationException : Exception
    {
        /// <summary>
        /// ctor
        /// </summary>
        public CosmosDbConfigurationException()
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        public CosmosDbConfigurationException(string message) : base(message)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        public CosmosDbConfigurationException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        protected CosmosDbConfigurationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
