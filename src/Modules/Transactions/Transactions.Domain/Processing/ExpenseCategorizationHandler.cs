namespace Transactions.Domain.Processing;

public class ExpenseCategorizationHandler : BaseTransactionHandler
{
    private readonly Dictionary<string[], string> _merchantCategories = new()
    {
        { new[] { "walmart", "target", "kroger", "safeway", "whole foods", "trader joe" }, "Groceries" },
        { new[] { "shell", "exxon", "chevron", "bp", "mobil", "gas station" }, "Transportation" },
        { new[] { "netflix", "spotify", "hulu", "disney", "amazon prime" }, "Entertainment" },
        { new[] { "starbucks", "mcdonald", "pizza", "restaurant", "cafe", "diner" }, "Dining Out" },
        { new[] { "electric", "gas company", "water", "internet", "phone", "utility" }, "Utilities" },
        { new[] { "pharmacy", "cvs", "walgreens", "hospital", "clinic", "medical" }, "Healthcare" },
        { new[] { "amazon", "ebay", "online", "shopping" }, "Shopping" },
        { new[] { "rent", "mortgage", "property", "landlord" }, "Housing" }
    };

    protected override async Task<TransactionProcessingResult> ProcessTransaction(BankTransactionRequest request)
    {
        await Task.Delay(15);

        // Only process debit transactions (expenses)
        if (request.TransactionType?.ToUpper() != "DEBIT" || request.Amount <= 0)
        {
            return new TransactionProcessingResult
            {
                IsProcessed = false,
                ProcessedBy = nameof(ExpenseCategorizationHandler),
                ProcessingNotes = { "Not a debit transaction" }
            };
        }

        var description = request.Description?.ToLower() ?? "";
        var merchant = request.MerchantName?.ToLower() ?? "";
        var fullText = $"{description} {merchant}";

        // Find matching category based on merchant/description keywords
        foreach (var (keywords, category) in _merchantCategories)
        {
            if (keywords.Any(keyword => fullText.Contains(keyword)))
            {
                return new TransactionProcessingResult
                {
                    IsProcessed = true,
                    CategoryName = category,
                    Classification = TransactionClassification.Expense,
                    ProcessedBy = nameof(ExpenseCategorizationHandler),
                    ProcessingNotes = { $"Categorized as {category} based on merchant/description matching" }
                };
            }
        }

        // Amount-based categorization for uncategorized expenses
        if (request.Amount > 1000m)
        {
            return new TransactionProcessingResult
            {
                IsProcessed = true,
                CategoryName = "Large Purchases",
                Classification = TransactionClassification.Expense,
                ProcessedBy = nameof(ExpenseCategorizationHandler),
                ProcessingNotes = { $"Large expense (${request.Amount:F2}) - requires manual review" }
            };
        }

        return new TransactionProcessingResult
        {
            IsProcessed = false,
            ProcessedBy = nameof(ExpenseCategorizationHandler),
            ProcessingNotes = { "Could not categorize expense" }
        };
    }
}