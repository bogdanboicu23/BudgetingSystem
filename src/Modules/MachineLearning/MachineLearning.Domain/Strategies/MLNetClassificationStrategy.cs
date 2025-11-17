using MachineLearning.Domain.Entities;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace MachineLearning.Domain.Strategies;

public class MlNetClassificationStrategy : ITransactionClassificationStrategy
{
    private MLContext _mlContext;
    private ITransformer? _trainedModel;
    private PredictionEngine<TransactionMlData, TransactionPrediction>? _predictionEngine;

    public string StrategyName => "ML.NET Classification";
    public bool IsModelTrained => _trainedModel != null;
    public DateTime? LastTrainingDate { get; private set; }

    public MlNetClassificationStrategy()
    {
        _mlContext = new MLContext(seed: 42);
    }

    public async Task<TransactionCategorization> ClassifyAsync(TransactionFeatures features)
    {
        if (!IsModelTrained)
            throw new InvalidOperationException("Model must be trained before classification");

        var mlData = ConvertToMlData(features);
        var prediction = _predictionEngine!.Predict(mlData);

        var result = new TransactionCategorization
        {
            PredictedType = features.Amount > 0 ? TransactionType.Income : TransactionType.Expense,
            PredictedCategory = prediction.PredictedCategory,
            PredictedSubcategory = prediction.PredictedSubcategory,
            Confidence = prediction.Score?.Max() ?? 0.5,
            ModelUsed = StrategyName,
            PredictionDate = DateTime.Now
        };

        // Add alternative predictions from ML scores
        result.AlternativePredictions = GenerateAlternativePredictions(prediction);

        return result;
    }

    public async Task TrainModelAsync(IEnumerable<TransactionTrainingData> trainingData)
    {
        var mlTrainingData = trainingData.Select(ConvertTrainingToMlData);
        var dataView = _mlContext.Data.LoadFromEnumerable(mlTrainingData);

        // Define the training pipeline
        var pipeline = _mlContext.Transforms.Text.FeaturizeText("DescriptionFeatures", nameof(TransactionMlData.Description))
            .Append(_mlContext.Transforms.Text.FeaturizeText("MerchantFeatures", nameof(TransactionMlData.MerchantName)))
            .Append(_mlContext.Transforms.Concatenate("Features",
                "DescriptionFeatures", "MerchantFeatures",
                nameof(TransactionMlData.AmountLog), nameof(TransactionMlData.DayOfWeek),
                nameof(TransactionMlData.IsWeekend), nameof(TransactionMlData.IsRecurring)))
            .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(
                labelColumnName: nameof(TransactionMlData.Category),
                featureColumnName: "Features"))
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedCategory", "PredictedLabel"));

        // Train the model
        _trainedModel = pipeline.Fit(dataView);
        _predictionEngine = _mlContext.Model.CreatePredictionEngine<TransactionMlData, TransactionPrediction>(_trainedModel);
        LastTrainingDate = DateTime.Now;

        await Task.CompletedTask;
    }

    public async Task<double> EvaluateModelAsync(IEnumerable<TransactionTrainingData> testData)
    {
        if (!IsModelTrained)
            return 0.0;

        var mlTestData = testData.Select(ConvertTrainingToMlData);
        var testDataView = _mlContext.Data.LoadFromEnumerable(mlTestData);

        var predictions = _trainedModel!.Transform(testDataView);
        var metrics = _mlContext.MulticlassClassification.Evaluate(predictions,
            labelColumnName: nameof(TransactionMlData.Category));

        return metrics.MacroAccuracy;
    }

    private TransactionMlData ConvertToMlData(TransactionFeatures features)
    {
        return new TransactionMlData
        {
            Description = CleanText(features.Description),
            MerchantName = CleanText(features.MerchantName),
            AmountLog = (float)Math.Log((double)Math.Abs(features.Amount) + 1),
            DayOfWeek = features.DayOfWeek,
            IsWeekend = features.IsWeekend,
            IsRecurring = features.IsRecurring,
            Category = "" // Will be predicted
        };
    }

    private TransactionMlData ConvertTrainingToMlData(TransactionTrainingData trainingData)
    {
        var mlData = ConvertToMlData(trainingData.Features);
        mlData.Category = trainingData.ActualCategory;
        return mlData;
    }

    private string CleanText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        return text.ToLower()
            .Replace("*", "")
            .Replace("#", "")
            .Replace("  ", " ")
            .Trim();
    }

    private List<CategoryPrediction> GenerateAlternativePredictions(TransactionPrediction prediction)
    {
        var alternatives = new List<CategoryPrediction>();

        // ML.NET provides score array for multiclass
        if (prediction.Score?.Length > 1)
        {
            var categories = PredefinedCategories.Categories.Keys.ToArray();
            var sortedPredictions = prediction.Score
                .Select((score, index) => new { Score = score, Index = index })
                .OrderByDescending(x => x.Score)
                .Skip(1) // Skip the top prediction
                .Take(3);

            foreach (var pred in sortedPredictions)
            {
                if (pred.Index < categories.Length)
                {
                    var category = categories[pred.Index];
                    alternatives.Add(new CategoryPrediction
                    {
                        Category = category,
                        Subcategory = PredefinedCategories.Categories[category].FirstOrDefault() ?? "Other",
                        Confidence = pred.Score,
                        Reasoning = $"ML.NET alternative prediction (score: {pred.Score:F3})"
                    });
                }
            }
        }

        return alternatives;
    }
}

// ML.NET data structures
public class TransactionMlData
{
    [LoadColumn(0)]
    public string Description { get; set; } = "";

    [LoadColumn(1)]
    public string MerchantName { get; set; } = "";

    [LoadColumn(2)]
    public float AmountLog { get; set; }

    [LoadColumn(3)]
    public float DayOfWeek { get; set; }

    [LoadColumn(4)]
    public bool IsWeekend { get; set; }

    [LoadColumn(5)]
    public bool IsRecurring { get; set; }

    [LoadColumn(6)]
    public string Category { get; set; } = "";
}

public class TransactionPrediction
{
    [ColumnName("PredictedLabel")]
    public string PredictedCategory { get; set; } = "";

    public string PredictedSubcategory { get; set; } = "";

    public float[]? Score { get; set; }
}