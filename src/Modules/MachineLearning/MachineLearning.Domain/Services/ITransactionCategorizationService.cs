using MachineLearning.Domain.Entities;
using MachineLearning.Domain.Pipelines;

namespace MachineLearning.Domain.Services;

public interface ITransactionCategorizationService
{
    Task<TransactionCategorization> CategorizeTransactionAsync(RawTransactionData transaction);
    Task<List<TransactionCategorization>> CategorizeTransactionsBatchAsync(IEnumerable<RawTransactionData> transactions);
    Task TrainModelsAsync(IEnumerable<TransactionTrainingData> trainingData);
    Task<ModelPerformanceReport> EvaluateModelsAsync(IEnumerable<TransactionTrainingData> testData);
    Task ProvideFeedbackAsync(Guid transactionId, string correctCategory, string correctSubcategory);
    Task<List<string>> GetSuggestedCategoriesAsync(string partialText);
}

public class ModelPerformanceReport
{
    public Dictionary<string, double> ModelAccuracies { get; set; } = new();
    public string RecommendedModel { get; set; } = "";
    public DateTime EvaluationDate { get; set; }
    public int TestDataSize { get; set; }
    public Dictionary<string, CategoryPerformance> CategoryPerformance { get; set; } = new();
}

public class CategoryPerformance
{
    public string Category { get; set; } = "";
    public double Precision { get; set; }
    public double Recall { get; set; }
    public double F1Score { get; set; }
    public int SampleCount { get; set; }
}