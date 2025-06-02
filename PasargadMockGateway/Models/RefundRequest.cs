using System.ComponentModel.DataAnnotations;

namespace PasargadMockGateway.Models;

public class RefundRequest
{
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    public string Invoice { get; set; } = string.Empty;
    
    [Required]
    public string Pan { get; set; } = string.Empty;
    
    [Required]
    public string Type { get; set; } = string.Empty; // RefundOnline یا RefundOffline
    
    [Required]
    public string UrlId { get; set; } = string.Empty;
} 