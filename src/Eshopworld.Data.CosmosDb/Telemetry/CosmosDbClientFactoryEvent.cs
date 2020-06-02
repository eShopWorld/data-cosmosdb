using Eshopworld.Core;

namespace Eshopworld.Data.CosmosDb.Telemetry
{
    public class CosmosDbClientFactoryEvent : TelemetryEvent
    {
        public string Message { get; }
        public CosmosDbClientFactoryEvent(string message)
        {
            Message = message;
        }
    }
}
