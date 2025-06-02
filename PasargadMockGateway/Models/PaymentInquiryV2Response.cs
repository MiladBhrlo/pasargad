namespace PasargadMockGateway.Models;

public class PaymentInquiryV2Response
{
    public string ResultMsg { get; set; } = string.Empty;
    public int ResultCode { get; set; }
    public PaymentInquiryV2Data? Data { get; set; }
}

public class PaymentInquiryV2Data
{
    public int Status { get; set; }
    public string TrackId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public string HashedCardNumber { get; set; } = string.Empty;
    public string Invoice { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public string RequestDate { get; set; } = string.Empty;
} 