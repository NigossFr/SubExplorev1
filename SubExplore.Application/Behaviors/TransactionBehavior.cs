using MediatR;
using Microsoft.Extensions.Logging;

namespace SubExplore.Application.Behaviors;

/// <summary>
/// Pipeline behavior that wraps request handling in a database transaction.
/// Commits the transaction on success and rolls back on failure.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResponse">The type of response.</typeparam>
/// <remarks>
/// This behavior is a placeholder for future implementation.
/// Actual transaction management will be implemented when the database context is added.
/// The behavior will use IUnitOfWork or DbContext to manage transactions.
/// </remarks>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public TransactionBehavior(ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Pipeline handler for transaction management.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="next">The next handler in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response from the next handler.</returns>
    /// <remarks>
    /// Future implementation will:
    /// 1. Begin a transaction before calling next()
    /// 2. Commit the transaction if next() succeeds
    /// 3. Rollback the transaction if next() throws an exception
    /// </remarks>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation(
            "Begin transaction for {RequestName}",
            requestName);

        try
        {
            // TODO: Begin transaction when DbContext is implemented
            // await _dbContext.BeginTransactionAsync(cancellationToken);

            var response = await next();

            // TODO: Commit transaction when DbContext is implemented
            // await _dbContext.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Committed transaction for {RequestName}",
                requestName);

            return response;
        }
        catch (Exception ex)
        {
            // TODO: Rollback transaction when DbContext is implemented
            // await _dbContext.RollbackTransactionAsync(cancellationToken);

            _logger.LogError(
                ex,
                "Rolled back transaction for {RequestName}",
                requestName);

            throw;
        }
    }
}
