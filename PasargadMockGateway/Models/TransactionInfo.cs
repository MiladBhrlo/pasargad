namespace PasargadMockGateway.Models;

public class TransactionInfo
{
    public string PaymentCode { get; set; } = string.Empty;
    public string Invoice { get; set; } = string.Empty;
    public string TerminalNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CallbackApi { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
} 