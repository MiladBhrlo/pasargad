namespace PasargadMockGateway.Models;

public class PaymentInquiryResponse
{
    public string ResultMsg { get; set; } = string.Empty;
    public int ResultCode { get; set; }
    public PaymentInquiryData? Data { get; set; }
}

public class PaymentInquiryData
{
    public int Status { get; set; }
    public string TrackId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public string Invoice { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public string RequestDate { get; set; } = string.Empty;
} 