using System;
using System.Collections.Generic;

namespace MachineLearning.Domain.Entities;

public class TransactionFeatures
{
    // Raw transaction data
    public string Description { get; set; } = "";
    public string MerchantName { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Location { get; set; }
    public string? MerchantCategory { get; set; }

    // Extracted features for ML
    public string[] DescriptionTokens { get; set; } = Array.Empty<string>();
    public string[] MerchantTokens { get; set; } = Array.Empty<string>();
    public decimal AmountLog { get; set; }
    public bool IsWeekend { get; set; }
    public int DayOfWeek { get; set; }
    public int HourOfDay { get; set; }
    public bool IsRecurring { get; set; }
    public string AmountRange { get; set; } = "";
    public string FrequencyPattern { get; set; } = "";

    // Historical patterns
    public int SimilarTransactionsCount { get; set; }
    public string MostCommonCategoryForMerchant { get; set; } = "";
    public double MerchantCategoryConfidence { get; set; }
}

public class TransactionCategorization
{
    public Guid TransactionId { get; set; }
    public TransactionType PredictedType { get; set; } // Income/Expense
    public string PredictedCategory { get; set; } = "";
    public string PredictedSubcategory { get; set; } = "";
    public double Confidence { get; set; }
    public string ModelUsed { get; set; } = "";
    public DateTime PredictionDate { get; set; }

    // Alternative predictions
    public List<CategoryPrediction> AlternativePredictions { get; set; } = new();

    // User feedback for model improvement
    public string? UserCorrectedCategory { get; set; }
    public string? UserCorrectedSubcategory { get; set; }
    public bool IsUserVerified { get; set; }
}

public class CategoryPrediction
{
    public string Category { get; set; } = "";
    public string Subcategory { get; set; } = "";
    public double Confidence { get; set; }
    public string Reasoning { get; set; } = "";
}

public enum TransactionType
{
    Income,
    Expense
}

// Predefined categories for training
public static class PredefinedCategories
{
    public static readonly Dictionary<string, List<string>> Categories = new()
    {
        ["Food"] = new() { "Groceries", "Restaurants", "Fast Food", "Coffee", "Delivery" },
        ["Transportation"] = new() { "Gas", "Public Transit", "Parking", "Car Maintenance", "Uber/Taxi" },
        ["Shopping"] = new() { "Clothing", "Electronics", "Books", "Household Items", "Online Shopping" },
        ["Entertainment"] = new() { "Movies", "Concerts", "Games", "Sports", "Subscriptions" },
        ["Bills"] = new() { "Utilities", "Phone", "Internet", "Insurance", "Rent/Mortgage" },
        ["Health"] = new() { "Medical", "Pharmacy", "Gym", "Dental", "Vision" },
        ["Income"] = new() { "Salary", "Freelance", "Investment", "Refund", "Gift" },
        ["Transfer"] = new() { "Bank Transfer", "Savings", "Investment", "Payment" }
    };

    public static readonly Dictionary<string, string[]> MerchantKeywords = new()
    {
        ["Food"] = new[] { "restaurant", "cafe", "pizza", "burger", "grocery", "market", "food", "deli", "bakery" },
        ["Transportation"] = new[] { "shell", "exxon", "chevron", "bp", "uber", "lyft", "metro", "bus", "parking" },
        ["Shopping"] = new[] { "amazon", "walmart", "target", "costco", "store", "shop", "retail", "mall" },
        ["Entertainment"] = new[] { "netflix", "spotify", "theater", "cinema", "game", "entertainment", "music" },
        ["Bills"] = new[] { "electric", "gas", "water", "phone", "internet", "insurance", "rent", "mortgage" },
        ["Health"] = new[] { "medical", "hospital", "pharmacy", "doctor", "dental", "clinic", "gym", "fitness" }
    };
}

public class TransactionTrainingData
{
    public TransactionFeatures Features { get; set; } = new();
    public string ActualCategory { get; set; } = "";
    public string ActualSubcategory { get; set; } = "";
    public TransactionType ActualType { get; set; }
}