using ExpenseService.Application.Commands;
using ExpenseService.Application.DTOs;
using ExpenseService.Domain.Entities;
using ExpenseService.Domain.Repositories;
using ExpenseService.Domain.ValueObjects;

namespace ExpenseService.Application.Handlers;

public class CreateExpenseCommandHandler
{
    private readonly IExpenseRepository _expenseRepository;

    public CreateExpenseCommandHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<ExpenseDto> Handle(CreateExpenseCommand command)
    {
        var money = new Money(command.Amount, command.Currency);
        var userId = UserId.From(command.UserId);
        var budgetId = command.BudgetId.HasValue ? BudgetId.From(command.BudgetId.Value) : null;

        var expense = new Expense(
            userId,
            command.Description,
            money,
            command.Category,
            command.Date,
            budgetId);

        if (!string.IsNullOrWhiteSpace(command.Merchant))
            expense.AddMerchant(command.Merchant);

        if (!string.IsNullOrWhiteSpace(command.Notes))
            expense.AddNotes(command.Notes);

        var createdExpense = await _expenseRepository.AddAsync(expense);

        return new ExpenseDto
        {
            Id = createdExpense.Id.Value,
            UserId = createdExpense.UserId.Value,
            BudgetId = createdExpense.BudgetId?.Value,
            Description = createdExpense.Description,
            Amount = createdExpense.Amount.Amount,
            Currency = createdExpense.Amount.Currency,
            Category = createdExpense.Category,
            Date = createdExpense.Date,
            Merchant = createdExpense.Merchant,
            Notes = createdExpense.Notes,
            CreatedAt = createdExpense.CreatedAt,
            LastModifiedAt = createdExpense.LastModifiedAt
        };
    }
}