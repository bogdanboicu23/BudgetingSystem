using Transactions.Domain.Processing;

namespace Transactions.Application.Services;

public class BankTransactionProcessor
{
    private readonly IBankTransactionHandler _processingChain;

    public BankTransactionProcessor()
    {
        _processingChain = BuildProcessingChain();
    }

    public async Task<List<TransactionProcessingResult>> ProcessBankTransactionsAsync(
        List<BankTransactionRequest> transactions)
    {
        var results = new List<TransactionProcessingResult>();

        foreach (var transaction in transactions)
        {
            var result = await _processingChain.Handle(transaction);
            results.Add(result);

            // Log processing result
            Console.WriteLine($"Transaction: ${transaction.Amount:F2} at {transaction.MerchantName}");
            Console.WriteLine($"Categorized as: {result.CategoryName} ({result.Classification})");
            Console.WriteLine($"Processed by: {result.ProcessedBy}");
            Console.WriteLine();
        }

        return results;
    }

    private static IBankTransactionHandler BuildProcessingChain()
    {
        var transferHandler = new TransferDetectionHandler();
        var incomeHandler = new IncomeDetectionHandler();
        var expenseHandler = new ExpenseCategorizationHandler();
        var fallbackHandler = new FallbackHandler();

        // Chain: Transfer -> Income -> Expense -> Fallback
        transferHandler
            .SetNext(incomeHandler)
            .SetNext(expenseHandler)
            .SetNext(fallbackHandler);

        return transferHandler;
    }

    // Mock method to simulate fetching transactions from bank API
    public async Task<List<BankTransactionRequest>> FetchTransactionsFromBankAsync(string accountId)
    {
        await Task.Delay(100); // Simulate API call

        // Mock bank transactions
        return new List<BankTransactionRequest>
        {
            new()
            {
                Description = "PAYROLL DEPOSIT - COMPANY XYZ",
                Amount = 2500.00m,
                Date = DateTime.Now.AddDays(-1),
                MerchantName = "COMPANY XYZ",
                TransactionType = "CREDIT"
            },
            new()
            {
                Description = "WALMART SUPERCENTER #1234",
                Amount = 127.45m,
                Date = DateTime.Now.AddDays(-2),
                MerchantName = "WALMART",
                TransactionType = "DEBIT"
            },
            new()
            {
                Description = "NETFLIX MONTHLY SUBSCRIPTION",
                Amount = 15.99m,
                Date = DateTime.Now.AddDays(-3),
                MerchantName = "NETFLIX",
                TransactionType = "DEBIT"
            },
            new()
            {
                Description = "TRANSFER TO SAVINGS ACCOUNT",
                Amount = 500.00m,
                Date = DateTime.Now.AddDays(-1),
                MerchantName = "INTERNAL TRANSFER",
                TransactionType = "DEBIT"
            },
            new()
            {
                Description = "SHELL GAS STATION #5678",
                Amount = 45.20m,
                Date = DateTime.Now,
                MerchantName = "SHELL",
                TransactionType = "DEBIT"
            }
        };
    }
}