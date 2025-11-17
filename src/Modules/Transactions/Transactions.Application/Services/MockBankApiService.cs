using Transactions.Domain.Processing;

namespace Transactions.Application.Services;

public interface IBankApiService
{
    Task<List<BankTransactionRequest>> GetTransactionsAsync(string accountId, DateTime fromDate, DateTime toDate);
    Task<AccountBalance> GetAccountBalanceAsync(string accountId);
}

public class MockBankApiService : IBankApiService
{
    private readonly List<BankTransactionRequest> _mockTransactions;

    public MockBankApiService()
    {
        _mockTransactions = GenerateMockTransactions();
    }

    public async Task<List<BankTransactionRequest>> GetTransactionsAsync(string accountId, DateTime fromDate, DateTime toDate)
    {
        await Task.Delay(200); // Simulate API call delay

        return _mockTransactions
            .Where(t => t.Date >= fromDate && t.Date <= toDate)
            .OrderByDescending(t => t.Date)
            .ToList();
    }

    public async Task<AccountBalance> GetAccountBalanceAsync(string accountId)
    {
        await Task.Delay(100);

        return new AccountBalance
        {
            AccountId = accountId,
            CurrentBalance = 2847.63m,
            AvailableBalance = 2647.63m,
            AsOfDate = DateTime.Now
        };
    }

    private List<BankTransactionRequest> GenerateMockTransactions()
    {
        var transactions = new List<BankTransactionRequest>();
        var random = new Random(42); // Fixed seed for consistent results

        // Generate transactions for the last 30 days
        for (int i = 0; i < 30; i++)
        {
            var date = DateTime.Now.AddDays(-i);

            // Add some income transactions
            if (i == 1 || i == 15) // Bi-weekly salary
            {
                transactions.Add(new BankTransactionRequest
                {
                    Description = "DIRECT DEPOSIT - EMPLOYER PAYROLL",
                    Amount = 1850.00m + random.Next(-50, 50),
                    Date = date,
                    MerchantName = "ACME CORP",
                    TransactionType = "CREDIT",
                    AccountNumber = "****1234"
                });
            }

            // Add random daily expenses
            var dailyTransactions = random.Next(0, 4);
            for (int j = 0; j < dailyTransactions; j++)
            {
                transactions.Add(GenerateRandomExpense(date, random));
            }
        }

        // Add some specific recurring transactions
        transactions.AddRange(GenerateRecurringTransactions());

        return transactions.OrderByDescending(t => t.Date).ToList();
    }

    private BankTransactionRequest GenerateRandomExpense(DateTime date, Random random)
    {
        var expenseTypes = new[]
        {
            ("STARBUCKS #1234", "STARBUCKS", 4.50m, 12.99m),
            ("WALMART SUPERCENTER", "WALMART", 25.00m, 150.00m),
            ("SHELL GAS STATION", "SHELL", 35.00m, 65.00m),
            ("MCDONALD'S #5678", "MCDONALDS", 8.00m, 18.00m),
            ("AMAZON.COM", "AMAZON", 15.00m, 89.99m),
            ("TARGET STORE", "TARGET", 30.00m, 120.00m),
            ("NETFLIX", "NETFLIX", 15.99m, 15.99m),
            ("UBER RIDE", "UBER", 12.00m, 35.00m),
            ("CVS PHARMACY", "CVS", 8.50m, 45.00m),
            ("LOCAL RESTAURANT", "RESTAURANT", 25.00m, 85.00m)
        };

        var (description, merchant, minAmount, maxAmount) = expenseTypes[random.Next(expenseTypes.Length)];
        var amount = (decimal)(random.NextDouble() * (double)(maxAmount - minAmount) + (double)minAmount);

        return new BankTransactionRequest
        {
            Description = description,
            Amount = Math.Round(amount, 2),
            Date = date.AddHours(random.Next(8, 20)).AddMinutes(random.Next(0, 59)),
            MerchantName = merchant,
            TransactionType = "DEBIT",
            AccountNumber = "****1234"
        };
    }

    private List<BankTransactionRequest> GenerateRecurringTransactions()
    {
        return new List<BankTransactionRequest>
        {
            new()
            {
                Description = "MORTGAGE PAYMENT - BANK OF AMERICA",
                Amount = 1250.00m,
                Date = DateTime.Now.AddDays(-3),
                MerchantName = "MORTGAGE COMPANY",
                TransactionType = "DEBIT",
                AccountNumber = "****1234"
            },
            new()
            {
                Description = "ELECTRIC COMPANY - MONTHLY BILL",
                Amount = 89.45m,
                Date = DateTime.Now.AddDays(-5),
                MerchantName = "ELECTRIC UTILITY",
                TransactionType = "DEBIT",
                AccountNumber = "****1234"
            },
            new()
            {
                Description = "INTERNET SERVICE - COMCAST",
                Amount = 79.99m,
                Date = DateTime.Now.AddDays(-7),
                MerchantName = "COMCAST",
                TransactionType = "DEBIT",
                AccountNumber = "****1234"
            },
            new()
            {
                Description = "TRANSFER TO SAVINGS ACCOUNT",
                Amount = 500.00m,
                Date = DateTime.Now.AddDays(-2),
                MerchantName = "INTERNAL TRANSFER",
                TransactionType = "DEBIT",
                AccountNumber = "****1234"
            },
            new()
            {
                Description = "DIVIDEND PAYMENT - VANGUARD",
                Amount = 45.67m,
                Date = DateTime.Now.AddDays(-10),
                MerchantName = "VANGUARD",
                TransactionType = "CREDIT",
                AccountNumber = "****1234"
            }
        };
    }
}

public class AccountBalance
{
    public string AccountId { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal AvailableBalance { get; set; }
    public DateTime AsOfDate { get; set; }
}