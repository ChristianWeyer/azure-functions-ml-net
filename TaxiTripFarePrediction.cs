using Microsoft.ML.Runtime.Api;

namespace Serverless
{
    public class TaxiTripFarePrediction
    {
        [ColumnName("Score")]
        public float FareAmount;
    }
}
