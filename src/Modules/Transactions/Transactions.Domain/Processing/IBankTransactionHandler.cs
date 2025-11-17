namespace Transactions.Domain.Processing;

public interface IBankTransactionHandler
{
    IBankTransactionHandler SetNext(IBankTransactionHandler handler);
    Task<TransactionProcessingResult> Handle(BankTransactionRequest request);
}

public class BankTransactionRequest
{
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string MerchantName { get; set; }
    public string AccountNumber { get; set; }
    public string TransactionType { get; set; } // DEBIT, CREDIT, etc.
    public Dictionary<string, object> RawData { get; set; } = new();
}

public class TransactionProcessingResult
{
    public bool IsProcessed { get; set; }
    public string CategoryId { get; set; }
    public string CategoryName { get; set; }
    public TransactionClassification Classification { get; set; }
    public string ProcessedBy { get; set; }
    public List<string> ProcessingNotes { get; set; } = new();
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

public enum TransactionClassification
{
    Income,
    Expense,
    Transfer,
    Unknown
}