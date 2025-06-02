using System.ComponentModel.DataAnnotations;

namespace PasargadMockGateway.Models;

public class PurchaseRequest
{
    [Required]
    public string Invoice { get; set; } = string.Empty;
    
    [Required]
    public string InvoiceDate { get; set; } = string.Empty;
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    public string CallbackApi { get; set; } = string.Empty;
    
    public string? MobileNumber { get; set; }
    
    [Required]
    public string ServiceCode { get; set; } = string.Empty;
    
    [Required]
    public string ServiceType { get; set; } = string.Empty;
    
    [Required]
    public int TerminalNumber { get; set; }
    
    public string? Description { get; set; }
    
    public string? PayerMail { get; set; }
    
    public string? PayerName { get; set; }
    
    public string? Pans { get; set; }
    
    public string? NationalCode { get; set; }
    
    public string? PaymentCode { get; set; }
} 