namespace PasargadMockGateway.Models;

public class PurchaseResponse
{
    public string ResultMsg { get; set; } = string.Empty;
    public int ResultCode { get; set; }
    public PurchaseData? Data { get; set; }
}

public class PurchaseData
{
    public string UrlId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
} 