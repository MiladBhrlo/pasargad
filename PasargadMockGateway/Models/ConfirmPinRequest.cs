using System.ComponentModel.DataAnnotations;

namespace PasargadMockGateway.Models;

public class ConfirmPinRequest
{
    [Required]
    public string CardNumber { get; set; } = string.Empty;
    
    [Required]
    public string Cvv2 { get; set; } = string.Empty;
    
    [Required]
    public string ExpireMonth { get; set; } = string.Empty;
    
    [Required]
    public string ExpireYear { get; set; } = string.Empty;
    
    [Required]
    public string Invoice { get; set; } = string.Empty;
    
    [Required]
    public long MerchantNumber { get; set; }
    
    [Required]
    public string Pin2 { get; set; } = string.Empty;
    
    [Required]
    public long TerminalNumber { get; set; }
} 