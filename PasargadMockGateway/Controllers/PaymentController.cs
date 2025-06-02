using Microsoft.AspNetCore.Mvc;
using PasargadMockGateway.Models;

namespace PasargadMockGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentController : ControllerBase
{
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(ILogger<PaymentController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{urlId}")]
    public IActionResult Payment(string urlId)
    {
        // در محیط واقعی، اینجا باید اطلاعات تراکنش را از دیتابیس بخوانید
        // و در صورت نیاز اعتبارسنجی انجام دهید

        // برای مثال، اطلاعات تراکنش را از کوئری استرینگ می‌خوانیم
        var token = Request.Query["token"].ToString();
        var invoiceNumber = Request.Query["invoiceNumber"].ToString();
        var terminalCode = Request.Query["terminalCode"].ToString();
        var amount = decimal.Parse(Request.Query["amount"].ToString() ?? "0");
        var redirectUrl = Request.Query["redirectUrl"].ToString();

        // هدایت به صفحه پرداخت
        return RedirectToPage("/Payment", new
        {
            Token = token,
            InvoiceNumber = invoiceNumber,
            TerminalCode = terminalCode,
            Amount = amount,
            RedirectUrl = redirectUrl
        });
    }
} 