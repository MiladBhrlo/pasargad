using System.ComponentModel.DataAnnotations;

namespace PasargadMockGateway.Models;

public class PaymentInquiryV2Request
{
    [Required]
    public string InvoiceId { get; set; } = string.Empty;
} 