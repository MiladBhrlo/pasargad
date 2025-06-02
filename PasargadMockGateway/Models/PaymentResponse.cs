namespace PasargadMockGateway.Models;

public class PaymentResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
} 