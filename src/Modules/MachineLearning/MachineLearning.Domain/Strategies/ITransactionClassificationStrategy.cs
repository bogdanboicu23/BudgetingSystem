using MachineLearning.Domain.Entities;

namespace MachineLearning.Domain.Strategies;

public interface ITransactionClassificationStrategy
{
    string StrategyName { get; }
    Task<TransactionCategorization> ClassifyAsync(TransactionFeatures features);
    Task TrainModelAsync(IEnumerable<TransactionTrainingData> trainingData);
    Task<double> EvaluateModelAsync(IEnumerable<TransactionTrainingData> testData);
    bool IsModelTrained { get; }
    DateTime? LastTrainingDate { get; }
}