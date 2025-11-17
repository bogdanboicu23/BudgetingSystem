using Budgets.Application.Commands.CreateBudget;
using Budgets.Domain.Entities;
using Budgets.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace Budgets.Application.Tests.Commands.CreateBudget;

public class CreateBudgetCommandHandlerTests
{
    private readonly Mock<IBudgetRepository> _budgetRepositoryMock;
    private readonly CreateBudgetCommandHandler _handler;

    public CreateBudgetCommandHandlerTests()
    {
        _budgetRepositoryMock = new Mock<IBudgetRepository>();
        _handler = new CreateBudgetCommandHandler(_budgetRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateBudgetAndReturnId()
    {
        // Arrange
        var command = new CreateBudgetCommand(
            "Monthly Budget",
            new DateTime(2024, 1, 1),
            new DateTime(2024, 1, 31)
        );

        _budgetRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Budget>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        _budgetRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Budget>(b =>
                b.Name == "Monthly Budget" &&
                b.PeriodStart == new DateTime(2024, 1, 1) &&
                b.PeriodEnd == new DateTime(2024, 1, 31)
            ), CancellationToken.None),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateBudgetWithUniqueId()
    {
        // Arrange
        var command = new CreateBudgetCommand(
            "Test Budget",
            DateTime.Now,
            DateTime.Now.AddDays(30)
        );

        Budget capturedBudget = null;
        _budgetRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Budget>(), It.IsAny<CancellationToken>()))
            .Callback<Budget, CancellationToken>((budget, _) => capturedBudget = budget)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        capturedBudget.Should().NotBeNull();
        capturedBudget.Id.Should().Be(result);
        capturedBudget.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WithCancellationToken_ShouldPassTokenToRepository()
    {
        // Arrange
        var command = new CreateBudgetCommand(
            "Test Budget",
            DateTime.Now,
            DateTime.Now.AddDays(30)
        );
        var cancellationToken = new CancellationToken();

        _budgetRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Budget>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _budgetRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Budget>(), cancellationToken),
            Times.Once
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Valid Budget Name")]
    public async Task Handle_WithDifferentNames_ShouldCreateBudgetWithGivenName(string budgetName)
    {
        // Arrange
        var command = new CreateBudgetCommand(
            budgetName,
            DateTime.Now,
            DateTime.Now.AddDays(30)
        );

        Budget capturedBudget = null;
        _budgetRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Budget>(), It.IsAny<CancellationToken>()))
            .Callback<Budget, CancellationToken>((budget, _) => capturedBudget = budget)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedBudget.Should().NotBeNull();
        capturedBudget.Name.Should().Be(budgetName);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new CreateBudgetCommand(
            "Test Budget",
            DateTime.Now,
            DateTime.Now.AddDays(30)
        );

        _budgetRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Budget>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database error");
    }
}