
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.ML;
using System;
using System.Threading.Tasks;
using Microsoft.ML.Trainers;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;

namespace Serverless
{
    public static class TaxiFarePredictor
    {
        private static readonly string _datapath = Path.Combine(
            Environment.CurrentDirectory, "data", "taxi-fare-train.csv");
        private static readonly string _testdatapath = Path.Combine(
            Environment.CurrentDirectory, "data", "taxi-fare-test.csv");
        private static readonly string _modelpath = Path.Combine(
            Environment.CurrentDirectory, "data", "Model.zip");

        [FunctionName("TaxiFarePredictor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = null)]
            TaxiTrip trip,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger TaxiFarePredictor function processed a request.");

            if (typeof(Microsoft.ML.Runtime.Data.LoadTransform) == null ||
                typeof(Microsoft.ML.Runtime.Learners.LinearClassificationTrainer) == null ||
                typeof(Microsoft.ML.Runtime.Internal.CpuMath.SseUtils) == null ||
                typeof(Microsoft.ML.Runtime.FastTree.FastTree) == null)
            {
                log.Error("Error loading ML.NET");

                return new StatusCodeResult(500);
            }

            PredictionModel<TaxiTrip, TaxiTripFarePrediction> model = await Train();
            TaxiTripFarePrediction prediction = model.Predict(trip);

            log.Info(String.Format("Predicted fare: {0}", prediction.FareAmount));

            return new OkObjectResult(prediction);
        }

        private static async Task<PredictionModel<TaxiTrip, TaxiTripFarePrediction>> Train()
        {
            var pipeline = new LearningPipeline
            {
                new TextLoader(_datapath).CreateFrom<TaxiTrip>(useHeader: true, separator: ','),
                new ColumnCopier(("FareAmount", "Label")),
                new CategoricalOneHotVectorizer(
                    "VendorId",
                    "RateCode",
                    "PaymentType"),
                new ColumnConcatenator(
                    "Features",
                    "VendorId",
                    "RateCode",
                    "PassengerCount",
                    "TripDistance",
                    "PaymentType"),
                new FastTreeRegressor()
            };

            var model = pipeline.Train<TaxiTrip, TaxiTripFarePrediction>();

            await model.WriteAsync(_modelpath);

            return model;
        }
    }
}
