using MachineLearning.Domain.Entities;

namespace MachineLearning.Domain.Strategies;

public class RuleBasedClassificationStrategy : ITransactionClassificationStrategy
{
    public string StrategyName => "Rule-Based Classification";
    public bool IsModelTrained => true; // Rule-based doesn't need training
    public DateTime? LastTrainingDate => DateTime.Now; // Always ready

    public Task<TransactionCategorization> ClassifyAsync(TransactionFeatures features)
    {
        var result = new TransactionCategorization
        {
            ModelUsed = StrategyName,
            PredictionDate = DateTime.Now
        };

        // Determine transaction type based on amount
        result.PredictedType = features.Amount > 0 ? TransactionType.Income : TransactionType.Expense;

        // Apply rule-based classification
        var (category, subcategory, confidence, reasoning) = ClassifyByRules(features);

        result.PredictedCategory = category;
        result.PredictedSubcategory = subcategory;
        result.Confidence = confidence;

        // Add alternative predictions based on rules
        result.AlternativePredictions = GenerateAlternativePredictions(features);

        return Task.FromResult(result);
    }

    private (string category, string subcategory, double confidence, string reasoning) ClassifyByRules(TransactionFeatures features)
    {
        var description = features.Description.ToLower();
        var merchant = features.MerchantName.ToLower();
        var amount = Math.Abs(features.Amount);

        // Income detection rules
        if (features.Amount > 0)
        {
            if (description.Contains("salary") || description.Contains("payroll"))
                return ("Income", "Salary", 0.95, "Contains salary keywords");

            if (description.Contains("refund") || description.Contains("return"))
                return ("Income", "Refund", 0.90, "Contains refund keywords");

            if (description.Contains("interest") || description.Contains("dividend"))
                return ("Income", "Investment", 0.90, "Contains investment keywords");

            return ("Income", "Other", 0.70, "Positive amount");
        }

        // Expense categorization rules
        foreach (var categoryPair in PredefinedCategories.MerchantKeywords)
        {
            var categoryName = categoryPair.Key;
            var keywords = categoryPair.Value;

            foreach (var keyword in keywords)
            {
                if (description.Contains(keyword) || merchant.Contains(keyword))
                {
                    var subcategory = DetermineSubcategory(categoryName, description, merchant, amount);
                    var confidence = CalculateConfidence(description, merchant, keyword);
                    return (categoryName, subcategory, confidence, $"Matched keyword: {keyword}");
                }
            }
        }

        // Amount-based rules for unclear transactions
        if (amount > 1000)
        {
            if (features.IsRecurring)
                return ("Bills", "Rent/Mortgage", 0.75, "Large recurring amount");
            else
                return ("Shopping", "Electronics", 0.60, "Large one-time purchase");
        }

        if (amount < 10)
        {
            if (features.IsWeekend)
                return ("Entertainment", "Movies", 0.65, "Small weekend expense");
            else
                return ("Food", "Coffee", 0.65, "Small daily expense");
        }

        // Default fallback
        return ("Other", "Miscellaneous", 0.50, "No clear pattern detected");
    }

    private string DetermineSubcategory(string category, string description, string merchant, decimal amount)
    {
        if (!PredefinedCategories.Categories.ContainsKey(category))
            return "Other";

        var subcategories = PredefinedCategories.Categories[category];

        // Smart subcategory detection based on specific keywords
        foreach (var subcategory in subcategories)
        {
            var subcategoryKeywords = GetSubcategoryKeywords(subcategory);
            if (subcategoryKeywords.Any(k => description.Contains(k.ToLower()) || merchant.Contains(k.ToLower())))
            {
                return subcategory;
            }
        }

        // Amount-based subcategory selection for some categories
        if (category == "Food")
        {
            if (amount > 100) return "Groceries";
            if (amount > 20) return "Restaurants";
            return "Fast Food";
        }

        if (category == "Transportation")
        {
            if (amount > 50) return "Gas";
            if (amount > 20) return "Uber/Taxi";
            return "Public Transit";
        }

        // Default to first subcategory
        return subcategories.FirstOrDefault() ?? "Other";
    }

    private string[] GetSubcategoryKeywords(string subcategory)
    {
        return subcategory.ToLower() switch
        {
            "groceries" => new[] { "grocery", "market", "supermarket", "walmart", "target", "costco" },
            "restaurants" => new[] { "restaurant", "cafe", "diner", "bistro", "grill" },
            "fast food" => new[] { "mcdonald", "burger", "pizza", "subway", "kfc", "taco" },
            "gas" => new[] { "shell", "exxon", "chevron", "bp", "gas", "fuel" },
            "uber/taxi" => new[] { "uber", "lyft", "taxi", "cab" },
            "netflix" => new[] { "netflix", "streaming" },
            "gym" => new[] { "gym", "fitness", "workout" },
            _ => new[] { subcategory.ToLower() }
        };
    }

    private double CalculateConfidence(string description, string merchant, string matchedKeyword)
    {
        double baseConfidence = 0.70;

        // Higher confidence for exact merchant matches
        if (merchant.Contains(matchedKeyword))
            baseConfidence += 0.15;

        // Higher confidence for description matches
        if (description.Contains(matchedKeyword))
            baseConfidence += 0.10;

        // Bonus for multiple keyword matches
        var totalMatches = CountKeywordMatches(description + " " + merchant, matchedKeyword);
        if (totalMatches > 1)
            baseConfidence += 0.05;

        return Math.Min(0.95, baseConfidence);
    }

    private int CountKeywordMatches(string text, string keyword)
    {
        return text.ToLower().Split(' ')
            .Count(word => word.Contains(keyword) || keyword.Contains(word));
    }

    private List<CategoryPrediction> GenerateAlternativePredictions(TransactionFeatures features)
    {
        var alternatives = new List<CategoryPrediction>();
        var description = features.Description.ToLower();
        var merchant = features.MerchantName.ToLower();

        // Find other possible categories
        foreach (var categoryPair in PredefinedCategories.MerchantKeywords.Take(3))
        {
            var categoryName = categoryPair.Key;
            var keywords = categoryPair.Value;

            var matchScore = keywords.Count(k => description.Contains(k) || merchant.Contains(k));
            if (matchScore > 0)
            {
                alternatives.Add(new CategoryPrediction
                {
                    Category = categoryName,
                    Subcategory = PredefinedCategories.Categories[categoryName].FirstOrDefault() ?? "Other",
                    Confidence = 0.30 + (matchScore * 0.10),
                    Reasoning = $"Partial keyword match ({matchScore} keywords)"
                });
            }
        }

        return alternatives.OrderByDescending(a => a.Confidence).Take(3).ToList();
    }

    public Task TrainModelAsync(IEnumerable<TransactionTrainingData> trainingData)
    {
        // Rule-based strategy doesn't need training
        // Could potentially use training data to improve rules
        return Task.CompletedTask;
    }

    public Task<double> EvaluateModelAsync(IEnumerable<TransactionTrainingData> testData)
    {
        // Simple evaluation based on rule accuracy
        var correct = 0;
        var total = 0;

        foreach (var data in testData)
        {
            var prediction = ClassifyAsync(data.Features).Result;
            if (prediction.PredictedCategory == data.ActualCategory)
                correct++;
            total++;
        }

        return Task.FromResult(total > 0 ? (double)correct / total : 0.0);
    }
}