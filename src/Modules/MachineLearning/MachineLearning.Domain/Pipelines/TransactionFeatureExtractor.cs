using MachineLearning.Domain.Entities;
using System.Text.RegularExpressions;

namespace MachineLearning.Domain.Pipelines;

public class TransactionFeatureExtractor : ITransactionFeatureExtractor
{
    private readonly Dictionary<string, int> _merchantFrequency = new();
    private readonly Dictionary<string, string> _merchantCategoryMap = new();

    public async Task<TransactionFeatures> ExtractFeaturesAsync(RawTransactionData rawTransaction)
    {
        var features = new TransactionFeatures
        {
            Description = rawTransaction.Description,
            MerchantName = rawTransaction.MerchantName,
            Amount = rawTransaction.Amount,
            Date = rawTransaction.Date,
            Location = rawTransaction.Location,
            MerchantCategory = rawTransaction.MerchantCategory
        };

        // Extract text features
        features.DescriptionTokens = ExtractTokens(rawTransaction.Description);
        features.MerchantTokens = ExtractTokens(rawTransaction.MerchantName);

        // Extract temporal features
        ExtractTemporalFeatures(features);

        // Extract amount features
        ExtractAmountFeatures(features);

        // Extract recurring pattern features
        await ExtractRecurringPatternFeatures(features);

        // Extract merchant history features
        await ExtractMerchantHistoryFeatures(features);

        return features;
    }

    private string[] ExtractTokens(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Array.Empty<string>();

        // Clean and tokenize
        var cleaned = Regex.Replace(text.ToLower(), @"[^\w\s]", " ");
        var tokens = cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Remove common stop words
        var stopWords = new HashSet<string> { "the", "and", "or", "in", "at", "on", "for", "to", "of", "with" };
        return tokens.Where(t => !stopWords.Contains(t) && t.Length > 2).ToArray();
    }

    private void ExtractTemporalFeatures(TransactionFeatures features)
    {
        features.DayOfWeek = (int)features.Date.DayOfWeek;
        features.IsWeekend = features.Date.DayOfWeek == DayOfWeek.Saturday ||
                            features.Date.DayOfWeek == DayOfWeek.Sunday;
        features.HourOfDay = features.Date.Hour;
    }

    private void ExtractAmountFeatures(TransactionFeatures features)
    {
        var absAmount = Math.Abs(features.Amount);
        features.AmountLog = (decimal)Math.Log((double)absAmount + 1);

        // Categorize amount ranges
        features.AmountRange = absAmount switch
        {
            < 10 => "Small",
            < 50 => "Medium",
            < 200 => "Large",
            < 1000 => "Very Large",
            _ => "Huge"
        };
    }

    private async Task ExtractRecurringPatternFeatures(TransactionFeatures features)
    {
        // Simple heuristic for detecting recurring transactions
        // In a real implementation, this would query historical data

        var recurringKeywords = new[] { "subscription", "monthly", "recurring", "auto", "payment" };
        var isRecurringKeyword = recurringKeywords.Any(k =>
            features.Description.ToLower().Contains(k) ||
            features.MerchantName.ToLower().Contains(k));

        // Check amount patterns (round numbers often indicate recurring payments)
        var isRoundAmount = features.Amount % 1 == 0 &&
                           (features.Amount % 5 == 0 || features.Amount % 10 == 0);

        features.IsRecurring = isRecurringKeyword || (isRoundAmount && Math.Abs(features.Amount) > 20);

        features.FrequencyPattern = features.IsRecurring ? "Monthly" : "One-time";

        await Task.CompletedTask;
    }

    private async Task ExtractMerchantHistoryFeatures(TransactionFeatures features)
    {
        var merchantKey = features.MerchantName.ToLower().Trim();

        // Track merchant frequency
        _merchantFrequency[merchantKey] = _merchantFrequency.GetValueOrDefault(merchantKey, 0) + 1;
        features.SimilarTransactionsCount = _merchantFrequency[merchantKey];

        // Predict category based on merchant history
        if (_merchantCategoryMap.ContainsKey(merchantKey))
        {
            features.MostCommonCategoryForMerchant = _merchantCategoryMap[merchantKey];
            features.MerchantCategoryConfidence = 0.8; // High confidence for known merchants
        }
        else
        {
            // Try to infer category from merchant name
            var inferredCategory = InferCategoryFromMerchantName(features.MerchantName);
            if (!string.IsNullOrEmpty(inferredCategory))
            {
                features.MostCommonCategoryForMerchant = inferredCategory;
                features.MerchantCategoryConfidence = 0.6; // Medium confidence for inferred
                _merchantCategoryMap[merchantKey] = inferredCategory;
            }
            else
            {
                features.MostCommonCategoryForMerchant = "Unknown";
                features.MerchantCategoryConfidence = 0.1;
            }
        }

        await Task.CompletedTask;
    }

    private string InferCategoryFromMerchantName(string merchantName)
    {
        var name = merchantName.ToLower();

        foreach (var categoryPair in PredefinedCategories.MerchantKeywords)
        {
            var category = categoryPair.Key;
            var keywords = categoryPair.Value;

            if (keywords.Any(keyword => name.Contains(keyword)))
            {
                return category;
            }
        }

        // Additional merchant-specific inference
        if (name.Contains("atm") || name.Contains("cash")) return "Transfer";
        if (name.Contains("paypal") || name.Contains("venmo")) return "Transfer";
        if (name.Contains("amazon") || name.Contains("ebay")) return "Shopping";
        if (name.Contains("spotify") || name.Contains("netflix")) return "Entertainment";

        return "";
    }

    public void UpdateMerchantCategory(string merchantName, string category)
    {
        var merchantKey = merchantName.ToLower().Trim();
        _merchantCategoryMap[merchantKey] = category;
    }

    public Dictionary<string, string> GetMerchantCategoryMap()
    {
        return new Dictionary<string, string>(_merchantCategoryMap);
    }
}