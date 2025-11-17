namespace Transactions.Domain.Processing;

public class TransferDetectionHandler : BaseTransactionHandler
{
    private readonly HashSet<string> _transferKeywords = new()
    {
        "transfer to", "transfer from", "online transfer", "wire transfer",
        "p2p", "venmo", "paypal", "zelle", "cashapp", "between accounts"
    };

    protected override async Task<TransactionProcessingResult> ProcessTransaction(BankTransactionRequest request)
    {
        await Task.Delay(10);

        var description = request.Description?.ToLower() ?? "";
        var merchant = request.MerchantName?.ToLower() ?? "";
        var fullText = $"{description} {merchant}";

        // Check for transfer keywords
        if (_transferKeywords.Any(keyword => fullText.Contains(keyword)))
        {
            return new TransactionProcessingResult
            {
                IsProcessed = true,
                CategoryName = "Account Transfers",
                Classification = TransactionClassification.Transfer,
                ProcessedBy = nameof(TransferDetectionHandler),
                ProcessingNotes = { "Detected as account transfer - excluded from budget calculations" }
            };
        }

        // Check for internal bank transfers (same account patterns)
        if (description.Contains("internal") ||
            description.Contains("savings") && description.Contains("checking"))
        {
            return new TransactionProcessingResult
            {
                IsProcessed = true,
                CategoryName = "Internal Transfers",
                Classification = TransactionClassification.Transfer,
                ProcessedBy = nameof(TransferDetectionHandler),
                ProcessingNotes = { "Internal bank transfer detected" }
            };
        }

        return new TransactionProcessingResult
        {
            IsProcessed = false,
            ProcessedBy = nameof(TransferDetectionHandler),
            ProcessingNotes = { "Not identified as a transfer" }
        };
    }
}