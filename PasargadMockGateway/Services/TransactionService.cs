using PasargadMockGateway.Models;

namespace PasargadMockGateway.Services;

public class TransactionService
{
    private readonly Dictionary<string, TransactionInfo> _transactions = new();
    private readonly ILogger<TransactionService> _logger;
    private readonly Timer _cleanupTimer;

    public TransactionService(ILogger<TransactionService> logger)
    {
        _logger = logger;
        // هر 5 دقیقه تراکنش‌های قدیمی‌تر از 30 دقیقه را پاک می‌کنیم
        _cleanupTimer = new Timer(CleanupOldTransactions, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    public void AddTransaction(string urlId, TransactionInfo info)
    {
        _transactions[urlId] = info;
    }

    public TransactionInfo? GetTransaction(string urlId)
    {
        return _transactions.TryGetValue(urlId, out var info) ? info : null;
    }

    private void CleanupOldTransactions(object? state)
    {
        var oldTransactions = _transactions
            .Where(x => (DateTime.Now - x.Value.CreatedAt).TotalMinutes > 30)
            .Select(x => x.Key)
            .ToList();

        foreach (var urlId in oldTransactions)
        {
            _transactions.Remove(urlId);
            _logger.LogInformation("Transaction {UrlId} removed due to timeout", urlId);
        }
    }
} 