using System.ComponentModel.DataAnnotations;

namespace PasargadMockGateway.Models;

public class VerifyRequest
{
    [Required]
    public string Invoice { get; set; } = string.Empty;
    
    [Required]
    public string UrlId { get; set; } = string.Empty;
} 