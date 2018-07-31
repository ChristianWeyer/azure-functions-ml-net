using Microsoft.ML.Runtime.Api;
using Newtonsoft.Json;

namespace Serverless
{
    public class TaxiTripFarePrediction
    {
        [JsonProperty("fareAmount")]
        [ColumnName("Score")]
        public float FareAmount;
    }
}
