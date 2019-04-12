using System;
using System.Collections.Generic;
using System.Text;

namespace Eshopworld.Data.CosmosDb.SessionHandling
{
    internal interface ICosmosDbSessionTokenProvider
    {
        string SessionToken { get; set; }
    }
}
