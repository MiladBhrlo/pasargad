using System.ComponentModel.DataAnnotations;

namespace PasargadMockGateway.Models;

public class PaymentInquiryRequest
{
    [Required]
    public string InvoiceId { get; set; } = string.Empty;
} 