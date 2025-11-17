using Microsoft.AspNetCore.Mvc;
using Transactions.Application.Services;
using Transactions.Domain.Processing;

namespace Webhost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly IBankApiService _bankApiService;
    private readonly IBankTransactionHandler _transactionHandler;

    public TransactionsController(
        IBankApiService bankApiService,
        IBankTransactionHandler transactionHandler)
    {
        _bankApiService = bankApiService;
        _transactionHandler = transactionHandler;
    }

    /// <summary>
    /// Process single transaction using Chain of Responsibility pattern
    /// </summary>
    [HttpPost("process-single")]
    public async Task<ActionResult> ProcessSingleTransaction([FromBody] SingleTransactionRequest request)
    {
        var transaction = new BankTransactionRequest
        {
            Description = request.Description,
            Amount = request.Amount,
            Date = request.Date,
            MerchantName = request.MerchantName,
            TransactionType = request.TransactionType
        };

        var result = await _transactionHandler.Handle(transaction);

        return Ok(new
        {
            Transaction = new
            {
                Description = transaction.Description,
                Amount = transaction.Amount,
                Date = transaction.Date,
                MerchantName = transaction.MerchantName,
                TransactionType = transaction.TransactionType
            },
            Result = new
            {
                CategoryName = result.CategoryName,
                Classification = result.Classification.ToString(),
                IsProcessed = result.IsProcessed,
                ProcessedBy = "Chain of Responsibility Pattern"
            }
        });
    }

    /// <summary>
    /// Process transactions using Chain of Responsibility pattern
    /// </summary>
    [HttpPost("process")]
    public async Task<ActionResult> ProcessTransactions([FromBody] ProcessTransactionsRequest request)
    {
        var transactions = await _bankApiService.GetTransactionsAsync(
            request.AccountId,
            request.FromDate,
            request.ToDate);

        var processedTransactions = new List<object>();
        var totalIncome = 0m;
        var totalExpenses = 0m;
        var totalTransfers = 0m;
        var categorizedCounts = new Dictionary<string, int>();
        var categoryTotals = new Dictionary<string, decimal>();

        foreach (var transaction in transactions)
        {
            var result = await _transactionHandler.Handle(transaction);

            processedTransactions.Add(new
            {
                Description = transaction.Description,
                Amount = transaction.Amount,
                Date = transaction.Date,
                MerchantName = transaction.MerchantName,
                TransactionType = transaction.TransactionType,
                CategoryName = result.CategoryName,
                Classification = result.Classification.ToString(),
                IsProcessed = result.IsProcessed
            });

            switch (result.Classification)
            {
                case TransactionClassification.Income:
                    totalIncome += transaction.Amount;
                    break;
                case TransactionClassification.Expense:
                    totalExpenses += transaction.Amount;
                    break;
                case TransactionClassification.Transfer:
                    totalTransfers += transaction.Amount;
                    break;
            }

            if (!categorizedCounts.ContainsKey(result.CategoryName))
            {
                categorizedCounts[result.CategoryName] = 0;
                categoryTotals[result.CategoryName] = 0;
            }
            categorizedCounts[result.CategoryName]++;
            categoryTotals[result.CategoryName] += transaction.Amount;
        }

        return Ok(new
        {
            ProcessedTransactions = processedTransactions,
            Summary = new
            {
                TotalTransactions = transactions.Count,
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                TotalTransfers = totalTransfers,
                CategorizedCounts = categorizedCounts,
                CategoryTotals = categoryTotals
            }
        });
    }

    /// <summary>
    /// Get mock transaction data for testing
    /// </summary>
    [HttpGet("mock-data/{accountId}")]
    public async Task<ActionResult> GetMockTransactions(
        string accountId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        fromDate ??= DateTime.Now.AddDays(-30);
        toDate ??= DateTime.Now;

        var transactions = await _bankApiService.GetTransactionsAsync(accountId, fromDate.Value, toDate.Value);

        return Ok(transactions.Select(t => new
        {
            Description = t.Description,
            Amount = t.Amount,
            Date = t.Date,
            MerchantName = t.MerchantName,
            TransactionType = t.TransactionType
        }));
    }
}

// DTOs
public class ProcessTransactionsRequest
{
    public string AccountId { get; set; } = "";
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

public class SingleTransactionRequest
{
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public string MerchantName { get; set; } = "";
    public string TransactionType { get; set; } = "";
}