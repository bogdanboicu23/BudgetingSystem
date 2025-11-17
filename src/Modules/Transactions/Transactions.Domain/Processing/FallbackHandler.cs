namespace Transactions.Domain.Processing;

public class FallbackHandler : BaseTransactionHandler
{
    protected override async Task<TransactionProcessingResult> ProcessTransaction(BankTransactionRequest request)
    {
        await Task.Delay(5);

        // Default classification based on transaction type
        var classification = request.TransactionType?.ToUpper() switch
        {
            "DEBIT" => TransactionClassification.Expense,
            "CREDIT" => TransactionClassification.Income,
            _ => TransactionClassification.Unknown
        };

        var categoryName = classification switch
        {
            TransactionClassification.Expense => "Uncategorized Expenses",
            TransactionClassification.Income => "Uncategorized Income",
            _ => "Unknown Transactions"
        };

        return new TransactionProcessingResult
        {
            IsProcessed = true,
            CategoryName = categoryName,
            Classification = classification,
            ProcessedBy = nameof(FallbackHandler),
            ProcessingNotes = {
                "Fallback categorization - manual review recommended",
                $"Transaction type: {request.TransactionType}",
                $"Amount: ${request.Amount:F2}",
                $"Description: {request.Description}"
            }
        };
    }
}