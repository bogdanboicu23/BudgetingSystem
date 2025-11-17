namespace Transactions.Domain.Processing;

public class IncomeDetectionHandler : BaseTransactionHandler
{
    private readonly HashSet<string> _incomeKeywords = new()
    {
        "salary", "payroll", "wage", "bonus", "dividend", "interest",
        "refund", "cashback", "freelance", "consulting", "transfer from"
    };

    protected override async Task<TransactionProcessingResult> ProcessTransaction(BankTransactionRequest request)
    {
        await Task.Delay(10); // Simulate processing time

        // Credit transactions are typically income
        if (request.TransactionType?.ToUpper() == "CREDIT" && request.Amount > 0)
        {
            var description = request.Description?.ToLower() ?? "";
            var merchant = request.MerchantName?.ToLower() ?? "";

            // Check for income keywords
            if (_incomeKeywords.Any(keyword => description.Contains(keyword) || merchant.Contains(keyword)))
            {
                var categoryName = DetermineIncomeCategory(description, merchant, request.Amount);

                return new TransactionProcessingResult
                {
                    IsProcessed = true,
                    CategoryName = categoryName,
                    Classification = TransactionClassification.Income,
                    ProcessedBy = nameof(IncomeDetectionHandler),
                    ProcessingNotes = { $"Detected as income based on keywords and credit type" }
                };
            }

            // Large credit amounts are likely income
            if (request.Amount > 500m)
            {
                return new TransactionProcessingResult
                {
                    IsProcessed = true,
                    CategoryName = "Other Income",
                    Classification = TransactionClassification.Income,
                    ProcessedBy = nameof(IncomeDetectionHandler),
                    ProcessingNotes = { $"Large credit amount (${request.Amount:F2}) classified as income" }
                };
            }
        }

        return new TransactionProcessingResult
        {
            IsProcessed = false,
            ProcessedBy = nameof(IncomeDetectionHandler),
            ProcessingNotes = { "Not classified as income" }
        };
    }

    private string DetermineIncomeCategory(string description, string merchant, decimal amount)
    {
        if (description.Contains("salary") || description.Contains("payroll"))
            return "Salary";

        if (description.Contains("dividend") || description.Contains("interest"))
            return "Investment Income";

        if (description.Contains("freelance") || description.Contains("consulting"))
            return "Freelance Income";

        if (description.Contains("refund") || description.Contains("cashback"))
            return "Refunds & Cashback";

        return "Other Income";
    }
}