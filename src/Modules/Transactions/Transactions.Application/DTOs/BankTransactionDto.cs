using Transactions.Domain.Processing;

namespace Transactions.Application.DTOs;

public class BankTransactionDto
{
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string MerchantName { get; set; }
    public string TransactionType { get; set; }
    public string CategoryName { get; set; }
    public string Classification { get; set; }
    public bool IsProcessed { get; set; }
}

public class ProcessTransactionsRequest
{
    public string AccountId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

public class ProcessTransactionsResponse
{
    public List<BankTransactionDto> ProcessedTransactions { get; set; } = new();
    public TransactionSummary Summary { get; set; }
}

public class TransactionSummary
{
    public int TotalTransactions { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalTransfers { get; set; }
    public Dictionary<string, int> CategorizedCounts { get; set; } = new();
    public Dictionary<string, decimal> CategoryTotals { get; set; } = new();
}