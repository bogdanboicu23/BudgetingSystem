using Budgets.Application.Queries.GetBudgetById;
using Budgets.Domain.Entities;
using Budgets.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace Budgets.Application.Tests.Queries.GetBudgetById;

public class GetBudgetByIdQueryHandlerTests
{
    private readonly Mock<IBudgetRepository> _budgetRepositoryMock;
    private readonly GetBudgetByIdQueryHandler _handler;

    public GetBudgetByIdQueryHandlerTests()
    {
        _budgetRepositoryMock = new Mock<IBudgetRepository>();
        _handler = new GetBudgetByIdQueryHandler(_budgetRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingBudget_ShouldReturnBudget()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var expectedBudget = new Budget(
            budgetId,
            "Test Budget",
            new DateTime(2024, 1, 1),
            new DateTime(2024, 1, 31)
        );

        var query = new GetBudgetByIdQuery(budgetId);

        _budgetRepositoryMock
            .Setup(x => x.GetByIdAsync(budgetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBudget);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedBudget);
        result.Id.Should().Be(budgetId);
        result.Name.Should().Be("Test Budget");
        result.PeriodStart.Should().Be(new DateTime(2024, 1, 1));
        result.PeriodEnd.Should().Be(new DateTime(2024, 1, 31));

        _budgetRepositoryMock.Verify(
            x => x.GetByIdAsync(budgetId, CancellationToken.None),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_NonExistingBudget_ShouldReturnNull()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var query = new GetBudgetByIdQuery(budgetId);

        _budgetRepositoryMock
            .Setup(x => x.GetByIdAsync(budgetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Budget?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _budgetRepositoryMock.Verify(
            x => x.GetByIdAsync(budgetId, CancellationToken.None),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithCancellationToken_ShouldPassTokenToRepository()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var query = new GetBudgetByIdQuery(budgetId);
        var cancellationToken = new CancellationToken();

        _budgetRepositoryMock
            .Setup(x => x.GetByIdAsync(budgetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Budget?)null);

        // Act
        await _handler.Handle(query, cancellationToken);

        // Assert
        _budgetRepositoryMock.Verify(
            x => x.GetByIdAsync(budgetId, cancellationToken),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_EmptyGuid_ShouldCallRepository()
    {
        // Arrange
        var emptyId = Guid.Empty;
        var query = new GetBudgetByIdQuery(emptyId);

        _budgetRepositoryMock
            .Setup(x => x.GetByIdAsync(emptyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Budget?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _budgetRepositoryMock.Verify(
            x => x.GetByIdAsync(emptyId, CancellationToken.None),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var query = new GetBudgetByIdQuery(budgetId);

        _budgetRepositoryMock
            .Setup(x => x.GetByIdAsync(budgetId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database connection error"));

        // Act & Assert
        await _handler.Invoking(h => h.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database connection error");
    }

    [Fact]
    public async Task Handle_BudgetWithCategories_ShouldReturnCompleteObject()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var budget = new Budget(
            budgetId,
            "Budget with Categories",
            new DateTime(2024, 1, 1),
            new DateTime(2024, 1, 31)
        );

        budget.AddCategory("Food", 500m);
        budget.AddCategory("Transportation", 200m);

        var query = new GetBudgetByIdQuery(budgetId);

        _budgetRepositoryMock
            .Setup(x => x.GetByIdAsync(budgetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(budget);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.BudgetCategories.Should().HaveCount(2);
        result.BudgetCategories.Should().Contain(c => c.Name == "Food");
        result.BudgetCategories.Should().Contain(c => c.Name == "Transportation");
    }
}