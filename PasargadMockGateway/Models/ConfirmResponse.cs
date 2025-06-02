namespace PasargadMockGateway.Models;

public class ConfirmResponse
{
    public string ResultMsg { get; set; } = string.Empty;
    public int ResultCode { get; set; }
    public ConfirmData? Data { get; set; }
}

public class ConfirmData
{
    public string Invoice { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public string TrackId { get; set; } = string.Empty;
    public string MaskedCardNumber { get; set; } = string.Empty;
    public string HashedCardNumber { get; set; } = string.Empty;
    public string RequestDate { get; set; } = string.Empty;
    public decimal Amount { get; set; }
} 