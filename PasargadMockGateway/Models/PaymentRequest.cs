using System.ComponentModel.DataAnnotations;

namespace PasargadMockGateway.Models;

public class PaymentRequest
{
    [Required]
    public string InvoiceNumber { get; set; } = string.Empty;
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    public string TerminalCode { get; set; } = string.Empty;
    
    [Required]
    public string MerchantCode { get; set; } = string.Empty;
    
    [Required]
    public string RedirectAddress { get; set; } = string.Empty;
    
    public string? Timestamp { get; set; }
    
    public string? Action { get; set; }
    
    public string? Sign { get; set; }
} 