using MachineLearning.Domain.Entities;
using MachineLearning.Domain.Pipelines;
using MachineLearning.Domain.Strategies;

namespace MachineLearning.Domain.Services;

public class TransactionCategorizationService : ITransactionCategorizationService
{
    private readonly ITransactionFeatureExtractor _featureExtractor;
    private readonly List<ITransactionClassificationStrategy> _strategies;
    private readonly Dictionary<Guid, TransactionCategorization> _feedback = new();

    public TransactionCategorizationService(
        ITransactionFeatureExtractor featureExtractor,
        IEnumerable<ITransactionClassificationStrategy> strategies)
    {
        _featureExtractor = featureExtractor;
        _strategies = strategies.ToList();
    }

    public async Task<TransactionCategorization> CategorizeTransactionAsync(RawTransactionData transaction)
    {
        // Extract features using pipeline
        var features = await _featureExtractor.ExtractFeaturesAsync(transaction);

        // Get predictions from all available strategies
        var predictions = new List<TransactionCategorization>();

        foreach (var strategy in _strategies.Where(s => s.IsModelTrained))
        {
            try
            {
                var prediction = await strategy.ClassifyAsync(features);
                prediction.TransactionId = transaction.Id;
                predictions.Add(prediction);
            }
            catch (Exception)
            {
                // Strategy failed, continue with others
                continue;
            }
        }

        if (!predictions.Any())
        {
            // Fallback to rule-based if no ML models are available
            var fallbackStrategy = _strategies.OfType<RuleBasedClassificationStrategy>().FirstOrDefault();
            if (fallbackStrategy != null)
            {
                var fallbackPrediction = await fallbackStrategy.ClassifyAsync(features);
                fallbackPrediction.TransactionId = transaction.Id;
                return fallbackPrediction;
            }

            throw new InvalidOperationException("No classification strategies available");
        }

        // Ensemble method: combine predictions
        return CombinePredictions(predictions, transaction.Id);
    }

    public async Task<List<TransactionCategorization>> CategorizeTransactionsBatchAsync(IEnumerable<RawTransactionData> transactions)
    {
        var results = new List<TransactionCategorization>();

        // Process in parallel for better performance
        var tasks = transactions.Select(async transaction =>
        {
            try
            {
                return await CategorizeTransactionAsync(transaction);
            }
            catch (Exception)
            {
                // Return a default categorization for failed transactions
                return new TransactionCategorization
                {
                    TransactionId = transaction.Id,
                    PredictedType = transaction.Amount > 0 ? TransactionType.Income : TransactionType.Expense,
                    PredictedCategory = "Other",
                    PredictedSubcategory = "Uncategorized",
                    Confidence = 0.1,
                    ModelUsed = "Fallback",
                    PredictionDate = DateTime.Now
                };
            }
        });

        results.AddRange(await Task.WhenAll(tasks));
        return results;
    }

    public async Task TrainModelsAsync(IEnumerable<TransactionTrainingData> trainingData)
    {
        var tasks = _strategies.Select(async strategy =>
        {
            try
            {
                await strategy.TrainModelAsync(trainingData);
            }
            catch (Exception ex)
            {
                // Log training failure but continue
                Console.WriteLine($"Training failed for {strategy.StrategyName}: {ex.Message}");
            }
        });

        await Task.WhenAll(tasks);
    }

    public async Task<ModelPerformanceReport> EvaluateModelsAsync(IEnumerable<TransactionTrainingData> testData)
    {
        var report = new ModelPerformanceReport
        {
            EvaluationDate = DateTime.Now,
            TestDataSize = testData.Count()
        };

        var tasks = _strategies.Where(s => s.IsModelTrained).Select(async strategy =>
        {
            try
            {
                var accuracy = await strategy.EvaluateModelAsync(testData);
                return new { Strategy = strategy.StrategyName, Accuracy = accuracy };
            }
            catch (Exception)
            {
                return new { Strategy = strategy.StrategyName, Accuracy = 0.0 };
            }
        });

        var results = await Task.WhenAll(tasks);

        foreach (var result in results)
        {
            report.ModelAccuracies[result.Strategy] = result.Accuracy;
        }

        report.RecommendedModel = report.ModelAccuracies
            .OrderByDescending(kv => kv.Value)
            .FirstOrDefault().Key ?? "Rule-Based";

        return report;
    }

    public async Task ProvideFeedbackAsync(Guid transactionId, string correctCategory, string correctSubcategory)
    {
        if (_feedback.ContainsKey(transactionId))
        {
            _feedback[transactionId].UserCorrectedCategory = correctCategory;
            _feedback[transactionId].UserCorrectedSubcategory = correctSubcategory;
            _feedback[transactionId].IsUserVerified = true;
        }

        // Use feedback to retrain models (simplified implementation)
        await Task.CompletedTask;
    }

    public async Task<List<string>> GetSuggestedCategoriesAsync(string partialText)
    {
        var suggestions = new List<string>();

        // Get categories that match the partial text
        var matchingCategories = PredefinedCategories.Categories.Keys
            .Where(c => c.ToLower().Contains(partialText.ToLower()))
            .ToList();

        suggestions.AddRange(matchingCategories);

        // Add subcategories that match
        foreach (var categoryPair in PredefinedCategories.Categories)
        {
            var matchingSubcategories = categoryPair.Value
                .Where(s => s.ToLower().Contains(partialText.ToLower()))
                .Select(s => $"{categoryPair.Key} > {s}");

            suggestions.AddRange(matchingSubcategories);
        }

        return suggestions.Take(10).ToList();
    }

    private TransactionCategorization CombinePredictions(List<TransactionCategorization> predictions, Guid transactionId)
    {
        // Weighted ensemble based on confidence scores
        var weightedPredictions = predictions
            .GroupBy(p => new { p.PredictedCategory, p.PredictedSubcategory })
            .Select(g => new
            {
                Category = g.Key.PredictedCategory,
                Subcategory = g.Key.PredictedSubcategory,
                WeightedConfidence = g.Sum(p => p.Confidence * GetStrategyWeight(p.ModelUsed)),
                Count = g.Count(),
                Models = string.Join(", ", g.Select(p => p.ModelUsed))
            })
            .OrderByDescending(p => p.WeightedConfidence)
            .ToList();

        var bestPrediction = weightedPredictions.First();

        // Create final result
        var result = new TransactionCategorization
        {
            TransactionId = transactionId,
            PredictedType = predictions.First().PredictedType,
            PredictedCategory = bestPrediction.Category,
            PredictedSubcategory = bestPrediction.Subcategory,
            Confidence = bestPrediction.WeightedConfidence / bestPrediction.Count,
            ModelUsed = $"Ensemble ({bestPrediction.Models})",
            PredictionDate = DateTime.Now
        };

        // Add alternatives
        result.AlternativePredictions = weightedPredictions.Skip(1).Take(3)
            .Select(p => new CategoryPrediction
            {
                Category = p.Category,
                Subcategory = p.Subcategory,
                Confidence = p.WeightedConfidence / p.Count,
                Reasoning = $"Ensemble alternative ({p.Models})"
            }).ToList();

        return result;
    }

    private double GetStrategyWeight(string strategyName)
    {
        return strategyName switch
        {
            "ML.NET Classification" => 1.0,      // Highest weight for trained ML
            "Rule-Based Classification" => 0.7,   // Medium weight for rules
            "Hybrid Classification" => 0.9,       // High weight for hybrid
            _ => 0.5                              // Default weight
        };
    }
}