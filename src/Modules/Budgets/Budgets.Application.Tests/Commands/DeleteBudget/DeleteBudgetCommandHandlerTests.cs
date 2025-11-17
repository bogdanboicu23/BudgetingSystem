using Budgets.Application.Commands.DeleteBudget;
using Budgets.Domain.Repositories;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace Budgets.Application.Tests.Commands.DeleteBudget;

public class DeleteBudgetCommandHandlerTests
{
    private readonly Mock<IBudgetRepository> _budgetRepositoryMock;
    private readonly DeleteBudgetCommandHandler _handler;

    public DeleteBudgetCommandHandlerTests()
    {
        _budgetRepositoryMock = new Mock<IBudgetRepository>();
        _handler = new DeleteBudgetCommandHandler(_budgetRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingBudget_ShouldDeleteBudgetSuccessfully()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var command = new DeleteBudgetCommand(budgetId);

        _budgetRepositoryMock
            .Setup(x => x.ExistsAsync(budgetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _budgetRepositoryMock
            .Setup(x => x.DeleteAsync(budgetId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        _budgetRepositoryMock.Verify(
            x => x.ExistsAsync(budgetId, CancellationToken.None),
            Times.Once
        );
        _budgetRepositoryMock.Verify(
            x => x.DeleteAsync(budgetId, CancellationToken.None),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_NonExistingBudget_ShouldThrowArgumentException()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var command = new DeleteBudgetCommand(budgetId);

        _budgetRepositoryMock
            .Setup(x => x.ExistsAsync(budgetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Budget with ID {budgetId} not found");

        _budgetRepositoryMock.Verify(
            x => x.ExistsAsync(budgetId, CancellationToken.None),
            Times.Once
        );
        _budgetRepositoryMock.Verify(
            x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_WithCancellationToken_ShouldPassTokenToRepository()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var command = new DeleteBudgetCommand(budgetId);
        var cancellationToken = new CancellationToken();

        _budgetRepositoryMock
            .Setup(x => x.ExistsAsync(budgetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _budgetRepositoryMock
            .Setup(x => x.DeleteAsync(budgetId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _budgetRepositoryMock.Verify(
            x => x.ExistsAsync(budgetId, cancellationToken),
            Times.Once
        );
        _budgetRepositoryMock.Verify(
            x => x.DeleteAsync(budgetId, cancellationToken),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_EmptyGuid_ShouldStillCallRepository()
    {
        // Arrange
        var emptyId = Guid.Empty;
        var command = new DeleteBudgetCommand(emptyId);

        _budgetRepositoryMock
            .Setup(x => x.ExistsAsync(emptyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ArgumentException>();

        _budgetRepositoryMock.Verify(
            x => x.ExistsAsync(emptyId, CancellationToken.None),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_RepositoryExistsThrowsException_ShouldPropagateException()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var command = new DeleteBudgetCommand(budgetId);

        _budgetRepositoryMock
            .Setup(x => x.ExistsAsync(budgetId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database connection error"));

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database connection error");
    }

    [Fact]
    public async Task Handle_RepositoryDeleteThrowsException_ShouldPropagateException()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var command = new DeleteBudgetCommand(budgetId);

        _budgetRepositoryMock
            .Setup(x => x.ExistsAsync(budgetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _budgetRepositoryMock
            .Setup(x => x.DeleteAsync(budgetId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Delete failed"));

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Delete failed");
    }
}