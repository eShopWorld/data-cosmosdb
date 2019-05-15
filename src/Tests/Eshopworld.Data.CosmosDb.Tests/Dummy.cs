using System;
using Newtonsoft.Json;

namespace Eshopworld.Data.CosmosDb.Tests
{
    internal class Dummy
    {
        [JsonProperty(PropertyName = "id")] public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
