namespace Transactions.Domain.Processing;

public abstract class BaseTransactionHandler : IBankTransactionHandler
{
    private IBankTransactionHandler _nextHandler;

    public IBankTransactionHandler SetNext(IBankTransactionHandler handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public virtual async Task<TransactionProcessingResult> Handle(BankTransactionRequest request)
    {
        var result = await ProcessTransaction(request);

        if (!result.IsProcessed && _nextHandler != null)
        {
            return await _nextHandler.Handle(request);
        }

        return result;
    }

    protected abstract Task<TransactionProcessingResult> ProcessTransaction(BankTransactionRequest request);
}