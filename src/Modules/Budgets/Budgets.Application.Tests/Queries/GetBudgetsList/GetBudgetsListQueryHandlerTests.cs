using Budgets.Application.Queries.GetBudgetsList;
using Budgets.Domain.Entities;
using Budgets.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace Budgets.Application.Tests.Queries.GetBudgetsList;

public class GetBudgetsListQueryHandlerTests
{
    private readonly Mock<IBudgetRepository> _budgetRepositoryMock;
    private readonly GetBudgetsListQueryHandler _handler;

    public GetBudgetsListQueryHandlerTests()
    {
        _budgetRepositoryMock = new Mock<IBudgetRepository>();
        _handler = new GetBudgetsListQueryHandler(_budgetRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBudgetsExist_ShouldReturnAllBudgets()
    {
        // Arrange
        var expectedBudgets = new List<Budget>
        {
            new Budget(Guid.NewGuid(), "Budget 1", new DateTime(2024, 1, 1), new DateTime(2024, 1, 31)),
            new Budget(Guid.NewGuid(), "Budget 2", new DateTime(2024, 2, 1), new DateTime(2024, 2, 28)),
            new Budget(Guid.NewGuid(), "Budget 3", new DateTime(2024, 3, 1), new DateTime(2024, 3, 31))
        };

        var query = new GetBudgetsListQuery();

        _budgetRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBudgets);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(expectedBudgets);

        _budgetRepositoryMock.Verify(
            x => x.GetAllAsync(CancellationToken.None),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WhenNoBudgetsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetBudgetsListQuery();

        _budgetRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Budget>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _budgetRepositoryMock.Verify(
            x => x.GetAllAsync(CancellationToken.None),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithCancellationToken_ShouldPassTokenToRepository()
    {
        // Arrange
        var query = new GetBudgetsListQuery();
        var cancellationToken = new CancellationToken();

        _budgetRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Budget>());

        // Act
        await _handler.Handle(query, cancellationToken);

        // Assert
        _budgetRepositoryMock.Verify(
            x => x.GetAllAsync(cancellationToken),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var query = new GetBudgetsListQuery();

        _budgetRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database connection error"));

        // Act & Assert
        await _handler.Invoking(h => h.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database connection error");
    }

    [Fact]
    public async Task Handle_BudgetsWithCategories_ShouldReturnCompleteObjects()
    {
        // Arrange
        var budget1 = new Budget(Guid.NewGuid(), "Budget 1", DateTime.Now, DateTime.Now.AddDays(30));
        budget1.AddCategory("Food", 500m);
        budget1.AddCategory("Transportation", 200m);

        var budget2 = new Budget(Guid.NewGuid(), "Budget 2", DateTime.Now, DateTime.Now.AddDays(30));
        budget2.AddCategory("Entertainment", 300m);

        var expectedBudgets = new List<Budget> { budget1, budget2 };
        var query = new GetBudgetsListQuery();

        _budgetRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBudgets);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var firstBudget = result.First(b => b.Name == "Budget 1");
        firstBudget.BudgetCategories.Should().HaveCount(2);
        firstBudget.BudgetCategories.Should().Contain(c => c.Name == "Food");
        firstBudget.BudgetCategories.Should().Contain(c => c.Name == "Transportation");

        var secondBudget = result.First(b => b.Name == "Budget 2");
        secondBudget.BudgetCategories.Should().HaveCount(1);
        secondBudget.BudgetCategories.Should().Contain(c => c.Name == "Entertainment");
    }

    [Fact]
    public async Task Handle_LargeBudgetList_ShouldHandleEfficiently()
    {
        // Arrange
        var largeBudgetList = new List<Budget>();
        for (int i = 0; i < 1000; i++)
        {
            largeBudgetList.Add(new Budget(
                Guid.NewGuid(),
                $"Budget {i}",
                DateTime.Now.AddDays(i),
                DateTime.Now.AddDays(i + 30)
            ));
        }

        var query = new GetBudgetsListQuery();

        _budgetRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(largeBudgetList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1000);
        result.Should().BeEquivalentTo(largeBudgetList);
    }

    [Fact]
    public async Task Handle_ShouldReturnIListInterface()
    {
        // Arrange
        var budgets = new List<Budget>
        {
            new Budget(Guid.NewGuid(), "Test Budget", DateTime.Now, DateTime.Now.AddDays(30))
        };

        var query = new GetBudgetsListQuery();

        _budgetRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(budgets);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeAssignableTo<IList<Budget>>();
        result.Should().BeOfType<List<Budget>>();
    }
}