using Microsoft.ML.Runtime.Api;
using Newtonsoft.Json;

namespace Serverless
{
    public class TaxiTrip
    {
        [JsonProperty("vendorId")]
        [Column("0")]
        public string VendorId;

        [JsonProperty("rateCode")]
        [Column("1")]
        public string RateCode;

        [JsonProperty("passengerCount")]
        [Column("2")]
        public float PassengerCount;

        [JsonProperty("tripTime")]
        [Column("3")]
        public float TripTime;

        [JsonProperty("tripDistance")]
        [Column("4")]
        public float TripDistance;

        [JsonProperty("paymentType")]
        [Column("5")]
        public string PaymentType;

        [JsonProperty("fareAmount")]
        [Column("6")]
        public float FareAmount;
    }
}